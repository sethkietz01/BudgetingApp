using BudgetingApp.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    /// <summary>
    /// Validates a login attempt
    /// </summary>
    /// <param name="model">The username and password attempted</param>
    /// <returns>The Dashboard if the attempt is valid, or the login page otherwise</returns>
    [HttpPost]
    public async Task<IActionResult> Login(UserModel model)
    {
        if (ModelState.IsValid)
        {
            // Ensure both username and password have values
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.ErrorMessage = "Please enter both username and password.";
                ViewBag.IsLoginPage = true;
                return View();
            }

            // Query Firestore for a user with the provided username
            QuerySnapshot snapshot = await _firestoreDb.Collection(_usersCollection)
                .WhereEqualTo("username", model.Username)
                .Limit(1)
                .GetSnapshotAsync();

            // If the user exists, verify the password
            if (snapshot.Documents.Count > 0) {
                DocumentSnapshot userDocument = snapshot.Documents.First();
                if (userDocument.TryGetValue<string>("password", out var storedPassword))
                    if (BCrypt.Net.BCrypt.Verify(model.Password, storedPassword)) {
                        // Successful attempt
                        HttpContext.Session.SetString("Username", model.Username);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ViewBag.ErrorMessage = "Invalid password.";
                else
                    ViewBag.ErrorMessage = "User found, but password information is missing.";
            }
            else
                ViewBag.ErrorMessage = "Invalid username.";
        }

        // If authentication fails or model is invalid, return to the login view with an error message
        ViewBag.IsLoginPage = true;
        return View();
    }

    /// <summary>
    /// Creates a new account and updates the Firestore with the new account
    /// </summary>
    /// <param name="model">The username and password for the account</param>
    /// <returns>The login page</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAccount(UserModel model)
    {
        if (ModelState.IsValid) {
            // Check if a user with this username already exists
            QuerySnapshot existingUserSnapshot = await _firestoreDb.Collection(_usersCollection)
                .WhereEqualTo("username", model.Username)
                .Limit(1)
                .GetSnapshotAsync();

            // If the account already exists, redirect to login
            if (existingUserSnapshot.Documents.Count > 0) {
                ViewBag.ErrorMessage = "An account with this username already exists. Please choose a different username.";
                ViewBag.IsCreateAccountPage = true;
                return View("Login", model); 
            }
            else {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Dictionary for Firestore add
                Dictionary<string, object> newUser = new Dictionary<string, object>
                {
                    { "username", model.Username },
                    { "password", hashedPassword } 
                };

                try {
                    await _firestoreDb.Collection(_usersCollection).AddAsync(newUser);

                    // Create a new assets document for the user with default values
                    string newUsername = model.Username;
                    Dictionary<string, object> newAssetDocument = new Dictionary<string, object>
                    {
                        { "username", newUsername },
                        { "balance", 0.0 },
                        { "income", 0.0 },
                        { "rent", 0.0 },
                        { "carPayment", 0.0 },
                        { "groceries", 0.0 },
                        { "gas", 0.0 },
                        { "subscriptions", 0.0 }
                    };
                    await _firestoreDb.Collection(_assetsCollection).AddAsync(newAssetDocument);

                    // Account creation successful, redirect to login page
                    return RedirectToAction("Login");
                }
                catch (Exception ex) {
                    ViewBag.ErrorMessage = $"Error creating account: {ex.Message}";
                    return View("Login", model);
                }
            }
        }
        else
        {
            // If the model is not valid, return to the login page with validation errors
            ViewBag.IsCreateAccountPage = true;
            return View("Login", model);
        }
    }
}