using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BudgetingApp.Models;

public class AuthController : Controller
{
    private readonly FirestoreDb _firestoreDb;
    private readonly string _usersCollection = "Users";

    public AuthController(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult CreateAccount()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserModel model)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.ErrorMessage = "Please enter both username and password.";
                return View();
            }

            // Query Firestore for a user with the provided username
            QuerySnapshot snapshot = await _firestoreDb.Collection(_usersCollection)
                .WhereEqualTo("username", model.Username)
                .Limit(1)
                .GetSnapshotAsync();

            if (snapshot.Documents.Count > 0)
            {
                DocumentSnapshot userDocument = snapshot.Documents.First();
                if (userDocument.TryGetValue<string>("password", out var storedPassword))
                    if (BCrypt.Net.BCrypt.Verify(model.Password, storedPassword))
                        return RedirectToAction("Index", "Home");
                    else
                        ViewBag.ErrorMessage = "Invalid password.";
                else
                    ViewBag.ErrorMessage = "User found, but password information is missing.";
            }
            else
                ViewBag.ErrorMessage = "Invalid username.";
        }

        // If authentication fails or model is invalid, return to the login view with an error message
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(UserModel model)
    {
        if (ModelState.IsValid)
        {
            // **Check if a user with this username already exists**
            QuerySnapshot existingUserSnapshot = await _firestoreDb.Collection(_usersCollection)
                .WhereEqualTo("username", model.Username)
                .Limit(1)
                .GetSnapshotAsync();

            if (existingUserSnapshot.Documents.Count > 0)
            {
                ViewBag.ErrorMessage = "An account with this username already exists. Please choose a different username.";
                return View("Login", model); // Return to the login page with the error
            }
            else
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                Dictionary<string, object> newUser = new Dictionary<string, object>
                {
                    { "username", model.Username },
                    { "password", hashedPassword } // Store the hashed password
                    // Add other user details as needed (e.g., email)
                };

                try
                {
                    await _firestoreDb.Collection(_usersCollection).AddAsync(newUser);
                    // Account creation successful, you might want to redirect to a registration success page
                    return RedirectToAction("Login"); // Redirect back to login page
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error creating account: {ex.Message}";
                    return View("Login", model); // Return to the login page with the entered data
                }
            }
        }
        else
        {
            // If the model is not valid, return to the login page with validation errors
            return View("Login", model);
        }
    }
}