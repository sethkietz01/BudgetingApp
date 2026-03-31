using BudgetingApp.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using OtpNet; 
using QRCoder;

public class AuthController : Controller
{
    private readonly FirestoreDb _firestoreDb;
    private readonly string _usersCollection = "Users";
    private readonly string _assetsCollection = "Assets";

    public AuthController(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ViewBag.IsLoginPage = true;
        return View();
    }

    [HttpGet]
    public IActionResult CreateAccount()
    {
        ViewBag.IsCreateAccountPage = true;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserModel model)
    {
        // Check for username and password
        if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
        {
            ViewBag.ErrorMessage = "Please enter both username and password.";
            ViewBag.IsLoginPage = true;
            return View();
        }

        // Get the user data from the database
        QuerySnapshot snapshot = await _firestoreDb.Collection(_usersCollection)
            .WhereEqualTo("username", model.Username)
            .Limit(1)
            .GetSnapshotAsync();

        if (snapshot.Documents.Count > 0) // User found
        {
            DocumentSnapshot userDocument = snapshot.Documents.First();
            if (userDocument.TryGetValue<string>("password", out var storedPassword)) // Get the stored hashed password
            {
                if (BCrypt.Net.BCrypt.Verify(model.Password, storedPassword)) // Password is correct
                {
                    // Check if MFA is enabled for the user
                    userDocument.TryGetValue("mfaEnabled", out bool isMfaEnabled);

                    if (isMfaEnabled)
                    {
                        // Store the username in session to use during MFA verification
                        HttpContext.Session.SetString("PendingMfaUsername", model.Username);
                        return RedirectToAction("VerifyMfaLogin");
                    }

                    // Log the user in
                    HttpContext.Session.SetString("Username", model.Username);
                    return RedirectToAction("Index", "Home");
                }
                else // Password is incorrect
                    ViewBag.ErrorMessage = "Invalid password.";
            }
            else // Password field is missing in the database
                ViewBag.ErrorMessage = "User found, but password information is missing.";
        }
        else // User not found
            ViewBag.ErrorMessage = "Invalid username.";

        ViewBag.IsLoginPage = true;
        return View();
    }

    [HttpGet]
    public IActionResult VerifyMfaLogin()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("PendingMfaUsername")))
            return RedirectToAction("Login");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyMfaLogin(string code)
    {
        // Get the username from session
        var username = HttpContext.Session.GetString("PendingMfaUsername");

        // Check valild username
        if (string.IsNullOrEmpty(username))
            return RedirectToAction("Login");

        // Get the user's MFA secret key from the database
        QuerySnapshot snapshot = await _firestoreDb.Collection(_usersCollection)
            .WhereEqualTo("username", username)
            .Limit(1)
            .GetSnapshotAsync();

        var userDoc = snapshot.Documents.First();
        if (userDoc.TryGetValue<string>("mfaSecretKey", out var secretKey))
        {
            // Initialize TOTP with the secret key
            var totp = new Totp(Base32Encoding.ToBytes(secretKey));
            if (totp.VerifyTotp(code, out _)) // Code is valid
            {
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.Remove("PendingMfaUsername");
                return RedirectToAction("Index", "Home");
            }
        }

        ViewBag.ErrorMessage = "Invalid code. Please try again.";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(UserModel model)
    {
        if (ModelState.IsValid)
        {
            // See if the username already exists
            QuerySnapshot existingUserSnapshot = await _firestoreDb.Collection(_usersCollection)
                .WhereEqualTo("username", model.Username)
                .Limit(1)
                .GetSnapshotAsync();

            if (existingUserSnapshot.Documents.Count > 0) // The username was found
            {
                ViewBag.ErrorMessage = "An account with this username already exists.";
                ViewBag.IsCreateAccountPage = true;
                return View("Login", model);
            }
            else // The username was not found
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                Dictionary<string, object> newUser = new Dictionary<string, object>
                {
                    { "username", model.Username },
                    { "password", hashedPassword },
                    { "mfaEnabled", model.MfaEnabled ?? false },
                    { "mfaSecretKey", "" } 
                };

                try
                {
                    await _firestoreDb.Collection(_usersCollection).AddAsync(newUser);

                    Dictionary<string, object> newAssetDocument = new Dictionary<string, object>
                    {
                        { "username", model.Username },
                        { "balance", 0.0 }, { "income", 0.0 }, { "rent", 0.0 },
                        { "carPayment", 0.0 }, { "groceries", 0.0 }, { "gas", 0.0 },
                        { "subscriptions", 0.0 }
                    };
                    await _firestoreDb.Collection(_assetsCollection).AddAsync(newAssetDocument);

                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error creating account: {ex.Message}";
                    return View("Login", model);
                }
            }
        }
        ViewBag.IsCreateAccountPage = true;
        return View("Login", model);
    }

    [HttpGet]
    public IActionResult SetupMfa()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username)) 
            return RedirectToAction("Login");

        // Generate Secret
        byte[] key = KeyGeneration.GenerateRandomKey(20);
        string base32Secret = Base32Encoding.ToString(key);

        // Prepare QR Code
        string issuer = "BudgetingApp";
        string uri = $"otpauth://totp/{issuer}:{username}?secret={base32Secret}&issuer={issuer}";

        // Generate QR Code
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            ViewBag.QrCodeImage = $"data:image/png;base64,{Convert.ToBase64String(qrCode.GetGraphic(20))}";
        }

        ViewBag.SecretKey = base32Secret;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmMfaSetup(string verificationCode, string secretKey)
    {
        var username = HttpContext.Session.GetString("Username");

        // If the session timed out while they were scanning the QR
        if (string.IsNullOrEmpty(username))
        {
            TempData["ErrorMessage"] = "Session expired. Please log in again.";
            return RedirectToAction("Login");
        }

        // Initialize TOTP with the secret
        var totp = new Totp(Base32Encoding.ToBytes(secretKey));

        var window = new VerificationWindow(previous: 1, future: 1);
        bool isValid = totp.VerifyTotp(verificationCode, out long timeStepMatched, window);

        if (isValid)
        {
            try
            {
                var snapshot = await _firestoreDb.Collection(_usersCollection)
                    .WhereEqualTo("username", username).Limit(1).GetSnapshotAsync();

                if (snapshot.Documents.Count > 0)
                {
                    await snapshot.Documents[0].Reference.UpdateAsync(new Dictionary<string, object>
                    {
                        { "mfaEnabled", true },
                        { "mfaSecretKey", secretKey }
                    });

                    TempData["SuccessMessage"] = "MFA has been successfully enabled!";
                    return RedirectToAction("Settings", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Database error: " + ex.Message;
            }
        }
        else // If it fails display why
            TempData["ErrorMessage"] = "Invalid code. Please ensure your phone time is correct and try again.";

        // It failed so go back to the setup page
        return RedirectToAction("SetupMfa");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableMfa()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username)) 
            return RedirectToAction("Login");

        // Find the user in Firestore
        var snapshot = await _firestoreDb.Collection(_usersCollection)
            .WhereEqualTo("username", username)
            .Limit(1)
            .GetSnapshotAsync();

        if (snapshot.Documents.Count > 0)
        {
            DocumentReference userRef = snapshot.Documents[0].Reference;

            // Wipe the MFA settings
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "mfaEnabled", false },
                { "mfaSecretKey", "" } 
            };

            await userRef.UpdateAsync(updates);
            TempData["SuccessMessage"] = "Two-Factor Authentication has been disabled.";
        }
        else
            TempData["ErrorMessage"] = "User not found. Could not disable MFA.";

        return RedirectToAction("Settings", "Home");
    }
}