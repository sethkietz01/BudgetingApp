using BudgetingApp.Models;
using BudgetingApp.Services;
using Google.Apis.Logging;
using Google.Apis.Util;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace BudgetingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirestoreDb _firestoreDb;
        private readonly string _assetsCollection = "Assets";
        private readonly string _usersCollection = "Users";
        private readonly string _transactionsCollection = "Transactions";
        private readonly BudgetingApp.Services.IAssetService _assetService;

        public HomeController(ILogger<HomeController> logger, FirestoreDb firestoreDb, IAssetService assetService)
        {
            _logger = logger;
            _firestoreDb = firestoreDb;
            _assetService = assetService;
        }

        /// <summary>
        /// Gets and displays all data from the Asset document for the current user
        /// </summary>
        /// <returns>Assets data</returns>
        public async Task<IActionResult> Index()
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Get the Asset documenet for the current user
            QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            List<AssetModel> assets = new List<AssetModel>();

            // Go through each Asset document (there should only be one)
            foreach (DocumentSnapshot currentSnapshot in snapshot.Documents)
            {
                if (currentSnapshot.Exists)
                {
                    AssetModel asset = new AssetModel();

                    /*** Firestore can either store numbers as type long or double, so we need to make sure that we cast accordingly ***/

                    // Handle each numerical field with type checking
                    if (currentSnapshot.TryGetValue<double>("balance", out var balanceDouble))
                        asset.Balance = (double)balanceDouble;
                    else if (currentSnapshot.TryGetValue<long>("balance", out var balanceLong))
                        asset.Balance = (double)balanceLong;
                    else
                    {
                        // Handle the case where the field is missing or of an unexpected type
                        asset.Balance = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'balance' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("income", out var incomeDouble))
                        asset.Income = incomeDouble;
                    else if (currentSnapshot.TryGetValue<long>("income", out var incomeLong))
                        asset.Income = (double)incomeLong;
                    else
                    {
                        asset.Income = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'income' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("savings", out var savingsDouble))
                        asset.Savings = savingsDouble;
                    else if (currentSnapshot.TryGetValue<long>("savings", out var savingsLong))
                        asset.Savings = (double)savingsLong;
                    else
                    {
                        asset.Savings = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'savings' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("rent", out var rentDouble))
                        asset.Rent = rentDouble;
                    else if (currentSnapshot.TryGetValue<long>("rent", out var rentLong))
                        asset.Rent = (double)rentLong;
                    else
                    {
                        asset.Rent = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'rent' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("utilities", out var utilitiesDouble))
                        asset.Utilities = utilitiesDouble;
                    else if (currentSnapshot.TryGetValue<long>("utilities", out var utilitiesLong))
                        asset.Utilities = (double)utilitiesLong;
                    else
                    {
                        asset.Utilities = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'utilities' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("carPayment", out var carPaymentDouble))
                        asset.CarPayment = carPaymentDouble;
                    else if (currentSnapshot.TryGetValue<long>("carPayment", out var carPaymentLong))
                        asset.CarPayment = (double)carPaymentLong;
                    else
                    {
                        asset.CarPayment = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'carPayment' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("insurances", out var insurancesDouble))
                        asset.Insurances = insurancesDouble;
                    else if (currentSnapshot.TryGetValue<long>("insurances", out var insurancesLong))
                        asset.Insurances = (double)insurancesLong;
                    else
                    {
                        asset.Insurances = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'insurances' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("groceries", out var groceriesDouble))
                        asset.Groceries = groceriesDouble;
                    else if (currentSnapshot.TryGetValue<long>("groceries", out var groceriesLong))
                        asset.Groceries = (double)groceriesLong;
                    else
                    {
                        asset.Groceries = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'groceries' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("gas", out var gasDouble))
                        asset.Gas = gasDouble;
                    else if (currentSnapshot.TryGetValue<long>("gas", out var gasLong))
                        asset.Gas = (double)gasLong;
                    else
                    {
                        asset.Gas = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'gas' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("subscriptions", out var subscriptionsDouble))
                        asset.Subscriptions = subscriptionsDouble;
                    else if (currentSnapshot.TryGetValue<long>("subscriptions", out var subscriptionsLong))
                        asset.Subscriptions = (double)subscriptionsLong;
                    else
                    {
                        asset.Subscriptions = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'subscriptions' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("other", out var otherDouble))
                        asset.Other = otherDouble;
                    else if (currentSnapshot.TryGetValue<long>("other", out var otherLong))
                        asset.Other = (double)otherLong;
                    else
                    {
                        asset.Other = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'other' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<string>("username", out var username))
                        asset.Username = username;
                    else
                    {
                        asset.Username = string.Empty;
                        _logger.LogWarning($"Unexpected data type for 'username' in document {currentSnapshot.Id}");
                    }

                    // If everything is good, add the asset to the list
                    assets.Add(asset);
                }
            }

            return View(assets);
        }

        /// <summary>
        /// Gets the Assets data for the current user to populate the edit list
        /// </summary>
        /// <returns>Assets data for the current user</returns>
        [HttpGet]
        public async Task<IActionResult> EditAssets()
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            AssetModel asset = new AssetModel();
            var currentSnapshot = snapshot.Documents[0];

            /*** Firestore can either store numbers as type long or double, so we need to make sure that we cast accordingly ***/

            // Handle each numerical field with type checking
            if (currentSnapshot.TryGetValue<double>("balance", out var balanceDouble))
                asset.Balance = (double)balanceDouble;
            else if (currentSnapshot.TryGetValue<long>("balance", out var balanceLong))
                asset.Balance = (double)balanceLong;
            else
            {
                // Handle the case where the field is missing or of an unexpected type
                asset.Balance = 0.0;
                _logger.LogWarning($"Unexpected data type for 'balance' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("income", out var incomeDouble))
                asset.Income = incomeDouble;
            else if (currentSnapshot.TryGetValue<long>("income", out var incomeLong))
                asset.Income = (double)incomeLong;
            else
            {
                asset.Income = 0.0;
                _logger.LogWarning($"Unexpected data type for 'income' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("savings", out var savingsDouble))
                asset.Savings = savingsDouble;
            else if (currentSnapshot.TryGetValue<long>("savings", out var savingsLong))
                asset.Savings = (double)savingsLong;
            else
            {
                asset.Savings = 0.0;
                _logger.LogWarning($"Unexpected data type for 'savings' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("rent", out var rentDouble))
                asset.Rent = rentDouble;
            else if (currentSnapshot.TryGetValue<long>("rent", out var rentLong))
                asset.Rent = (double)rentLong;
            else
            {
                asset.Rent = 0.0;
                _logger.LogWarning($"Unexpected data type for 'rent' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("utilities", out var utilitiesDouble))
                asset.Utilities = utilitiesDouble;
            else if (currentSnapshot.TryGetValue<long>("utilities", out var utilitiesLong))
                asset.Utilities = (double)utilitiesLong;
            else
            {
                asset.Utilities = 0.0;
                _logger.LogWarning($"Unexpected data type for 'utilities' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("carPayment", out var carPaymentDouble))
                asset.CarPayment = carPaymentDouble;
            else if (currentSnapshot.TryGetValue<long>("carPayment", out var carPaymentLong))
                asset.CarPayment = (double)carPaymentLong;
            else
            {
                asset.CarPayment = 0.0;
                _logger.LogWarning($"Unexpected data type for 'carPayment' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("insurances", out var insurancesDouble))
                asset.Insurances = insurancesDouble;
            else if (currentSnapshot.TryGetValue<long>("insurances", out var insurancesLong))
                asset.Insurances = (double)insurancesLong;
            else
            {
                asset.Insurances = 0.0;
                _logger.LogWarning($"Unexpected data type for 'insurances' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("groceries", out var groceriesDouble))
                asset.Groceries = groceriesDouble;
            else if (currentSnapshot.TryGetValue<long>("groceries", out var groceriesLong))
                asset.Groceries = (double)groceriesLong;
            else
            {
                asset.Groceries = 0.0;
                _logger.LogWarning($"Unexpected data type for 'groceries' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("gas", out var gasDouble))
                asset.Gas = gasDouble;
            else if (currentSnapshot.TryGetValue<long>("gas", out var gasLong))
                asset.Gas = (double)gasLong;
            else
            {
                asset.Gas = 0.0;
                _logger.LogWarning($"Unexpected data type for 'gas' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("subscriptions", out var subscriptionsDouble))
                asset.Subscriptions = subscriptionsDouble;
            else if (currentSnapshot.TryGetValue<long>("subscriptions", out var subscriptionsLong))
                asset.Subscriptions = (double)subscriptionsLong;
            else
            {
                asset.Subscriptions = 0.0;
                _logger.LogWarning($"Unexpected data type for 'subscriptions' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<double>("other", out var otherDouble))
                asset.Other = otherDouble;
            else if (currentSnapshot.TryGetValue<long>("other", out var otherLong))
                asset.Other = (double)otherLong;
            else
            {
                asset.Other = 0.0;
                _logger.LogWarning($"Unexpected data type for 'other' in document {currentSnapshot.Id}");
            }

            if (currentSnapshot.TryGetValue<string>("username", out var username))
                asset.Username = username;
            else
            {
                asset.Username = string.Empty;
                _logger.LogWarning($"Unexpected data type for 'username' in document {currentSnapshot.Id}");
            }

            if (asset == null)
                return NotFound();

            return View(asset);
        }

        /// <summary>
        /// Updates the Asset document for the current user in the database based on the passed AssetModel
        /// </summary>
        /// <param name="model">The new Asset data</param>
        /// <returns>The page with the new Asset data</returns>
        [HttpPost]
        public async Task<IActionResult> EditAssets(AssetModel model)
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (currentUser == null || currentUser == "")
                return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                // Find the existing document for the current user
                QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                    .WhereEqualTo("username", currentUser)
                    .GetSnapshotAsync();

                DocumentSnapshot assetDocument = snapshot.Documents.FirstOrDefault();

                if (assetDocument != null)
                {
                    DocumentReference assetRef = _firestoreDb.Collection(_assetsCollection).Document(assetDocument.Id);
                    Dictionary<string, object> updates = new Dictionary<string, object>();

                    // Conditionally update fields if a value was provided in the form
                    if (!string.IsNullOrEmpty(Request.Form["Balance"]))
                    {
                        if (double.TryParse(Request.Form["Balance"], out double balance))
                            updates["balance"] = balance;
                        else
                        {
                            ModelState.AddModelError("Balance", "The Balance must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Income"]))
                    {
                        if (double.TryParse(Request.Form["Income"], out double income))
                            updates["income"] = income;
                        else
                        {
                            ModelState.AddModelError("Income", "The Income must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Savings"]))
                    {
                        if (double.TryParse(Request.Form["Savings"], out double savings))
                            updates["savings"] = savings;
                        else
                        {
                            ModelState.AddModelError("Savings", "The Savings must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Rent"]))
                    {
                        if (double.TryParse(Request.Form["Rent"], out double rent))
                            updates["rent"] = rent;
                        else
                        {
                            ModelState.AddModelError("Rent", "The Rent must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Utilities"]))
                    {
                        if (double.TryParse(Request.Form["Utilities"], out double utilities))
                            updates["utilities"] = utilities;
                        else
                        {
                            ModelState.AddModelError("Utilities", "The Utilities must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["CarPayment"]))
                    {
                        if (double.TryParse(Request.Form["CarPayment"], out double carPayment))
                            updates["carPayment"] = carPayment;
                        else
                        {
                            ModelState.AddModelError("CarPayment", "The Car Payment must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Insurances"]))
                    {
                        if (double.TryParse(Request.Form["Insurances"], out double insurances))
                            updates["insurances"] = insurances;
                        else
                        {
                            ModelState.AddModelError("Insurances", "The Insurances must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Groceries"]))
                    {
                        if (double.TryParse(Request.Form["Groceries"], out double groceries))
                            updates["groceries"] = groceries;
                        else
                        {
                            ModelState.AddModelError("Groceries", "The Groceries must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Gas"]))
                    {
                        if (double.TryParse(Request.Form["Gas"], out double gas))
                            updates["gas"] = gas;
                        else
                        {
                            ModelState.AddModelError("Gas", "The Gas must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Subscriptions"]))
                    {
                        if (double.TryParse(Request.Form["Subscriptions"], out double subscriptions))
                            updates["subscriptions"] = subscriptions;
                        else
                        {
                            ModelState.AddModelError("Subscriptions", "The Subscriptions must be a valid number.");
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.Form["Other"]))
                    {
                        if (double.TryParse(Request.Form["Other"], out double other))
                            updates["other"] = other;
                        else
                        {
                            ModelState.AddModelError("Other", "The Other category must be a valid number");
                            return View(model);
                        }
                    }

                    if (updates.Count > 0)
                    {
                        await assetRef.UpdateAsync(updates);
                        return RedirectToAction("Index");
                    }
                    else
                        return RedirectToAction("Index");
                }
                else
                    return NotFound();
            }

            // ModelState is not valid
            Console.WriteLine("The ModelState is NOT valid. Errors:");
            foreach (var keyValuePair in ModelState)
            {
                if (keyValuePair.Value.Errors.Count > 0)
                {
                    Console.WriteLine($"  {keyValuePair.Key}:");
                    foreach (var error in keyValuePair.Value.Errors)
                    {
                        Console.WriteLine($"    - {error.ErrorMessage}");
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// Error logging method
        /// </summary>
        /// <returns>The ErrorViewModel with error data</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Adds a passed amount to the balance in the database
        /// </summary>
        /// <param name="amountToAdd">The amount to add to the balance</param>
        /// <returns>The view with the updated amount</returns>
        [HttpPost]
        public async Task<IActionResult> AddToBalance(int amountToAdd)
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Query the database for the current user's Asset documents 
            QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
                return NotFound($"No asset document found for user: {currentUser}");

            // Get the user's Asset document
            DocumentSnapshot assetDocument = snapshot.Documents.First();
            DocumentReference assetReference = _firestoreDb.Collection(_assetsCollection).Document(assetDocument.Id);

            // Update the balance
            await assetReference.UpdateAsync("balance", FieldValue.Increment(amountToAdd));

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SubtractFromBalance(int amountToSubtract)
        {
            // Make the amount negative
            amountToSubtract = amountToSubtract * (-1);

            Console.WriteLine(amountToSubtract);

            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Query the database for the current user's Asset documents 
            QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
                return NotFound($"No asset document found for user: {currentUser}");
            // Get the user's Asset document
            DocumentSnapshot assetDocument = snapshot.Documents.First();
            DocumentReference assetReference = _firestoreDb.Collection(_assetsCollection).Document(assetDocument.Id);

            // Update the balance
            await assetReference.UpdateAsync("balance", FieldValue.Increment(amountToSubtract));

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gets the Asset data for the current user to use in a What If scenario
        /// </summary>
        /// <returns>The view with the user's Asset data</returns>
        public async Task<IActionResult> WhatIf()
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Query the database for the current user's Asset documents 
            QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            List<AssetModel> assets = new List<AssetModel>();
            foreach (DocumentSnapshot currentSnapshot in snapshot.Documents)
            {
                if (currentSnapshot.Exists)
                {
                    AssetModel asset = new AssetModel();

                    /*** Firestore can either store numbers as type long or double, so we need to make sure that we cast accordingly ***/

                    // Handle each numerical field with type checking
                    if (currentSnapshot.TryGetValue<double>("balance", out var balanceDouble))
                        asset.Balance = (double)balanceDouble;
                    else if (currentSnapshot.TryGetValue<long>("balance", out var balanceLong))
                        asset.Balance = (double)balanceLong;
                    else
                    {
                        // Handle the case where the field is missing or of an unexpected type
                        asset.Balance = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'balance' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("income", out var incomeDouble))
                        asset.Income = incomeDouble;
                    else if (currentSnapshot.TryGetValue<long>("income", out var incomeLong))
                        asset.Income = (double)incomeLong;
                    else
                    {
                        asset.Income = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'income' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("savings", out var savingsDouble))
                        asset.Savings = savingsDouble;
                    else if (currentSnapshot.TryGetValue<long>("savings", out var savingsLong))
                        asset.Savings = (double)savingsLong;
                    else
                    {
                        asset.Savings = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'savings' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("rent", out var rentDouble))
                        asset.Rent = rentDouble;
                    else if (currentSnapshot.TryGetValue<long>("rent", out var rentLong))
                        asset.Rent = (double)rentLong;
                    else
                    {
                        asset.Rent = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'rent' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("utilities", out var utilitiesDouble))
                        asset.Utilities = utilitiesDouble;
                    else if (currentSnapshot.TryGetValue<long>("utilities", out var utilitiesLong))
                        asset.Utilities = (double)utilitiesLong;
                    else
                    {
                        asset.Utilities = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'utilities' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("carPayment", out var carPaymentDouble))
                        asset.CarPayment = carPaymentDouble;
                    else if (currentSnapshot.TryGetValue<long>("carPayment", out var carPaymentLong))
                        asset.CarPayment = (double)carPaymentLong;
                    else
                    {
                        asset.CarPayment = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'carPayment' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("insurances", out var insurancesDouble))
                        asset.Insurances = insurancesDouble;
                    else if (currentSnapshot.TryGetValue<long>("insurances", out var insurancesLong))
                        asset.Insurances = (double)insurancesLong;
                    else
                    {
                        asset.Insurances = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'insurances' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("groceries", out var groceriesDouble))
                        asset.Groceries = groceriesDouble;
                    else if (currentSnapshot.TryGetValue<long>("groceries", out var groceriesLong))
                        asset.Groceries = (double)groceriesLong;
                    else
                    {
                        asset.Groceries = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'groceries' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("gas", out var gasDouble))
                        asset.Gas = gasDouble;
                    else if (currentSnapshot.TryGetValue<long>("gas", out var gasLong))
                        asset.Gas = (double)gasLong;
                    else
                    {
                        asset.Gas = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'gas' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("subscriptions", out var subscriptionsDouble))
                        asset.Subscriptions = subscriptionsDouble;
                    else if (currentSnapshot.TryGetValue<long>("subscriptions", out var subscriptionsLong))
                        asset.Subscriptions = (double)subscriptionsLong;
                    else
                    {
                        asset.Subscriptions = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'subscriptions' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("other", out var otherDouble))
                        asset.Other = otherDouble;
                    else if (currentSnapshot.TryGetValue<long>("other", out var otherLong))
                        asset.Other = (double)otherLong;
                    else
                    {
                        asset.Other = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'other' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<string>("username", out var username))
                        asset.Username = username;
                    else
                    {
                        asset.Username = string.Empty;
                        _logger.LogWarning($"Unexpected data type for 'username' in document {currentSnapshot.Id}");
                    }

                    assets.Add(asset);
                }
            }

            return View(assets);
        }

        /// <summary>
        /// Displays the Settings View
        /// </summary>
        /// <returns>The Settings View</returns>
        public IActionResult Settings()
        {
            return View();
        }

        /// <summary>
        /// Removes the users Session and redirects to the login page
        /// </summary>
        /// <returns>The Login View</returns>
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login", "Auth");
        }

        /// <summary>
        /// Displays the AddTransaction View
        /// </summary>
        /// <returns>The AddTranscation View</returns>
        public IActionResult AddTransaction()
        {
            return View();
        }

        /// <summary>
        /// Gets all Transaction data for the current user and displays the Transaction View
        /// </summary>
        /// <returns>The Transaction View with all of the user's Transactions</returns>
        [HttpGet]
        public async Task<IActionResult> Transactions(DateTime? filterStartDate, DateTime? filterEndDate, string? filterMerchant, string? filterCategory)
        {
            // Determine who is currently logged in
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");
            
            // Set up an intial query for the database
            Query query = _firestoreDb.Collection(_transactionsCollection)
                .WhereEqualTo("username", currentUser);

            // Check the date range
            if (filterStartDate.HasValue)
                query = query.WhereGreaterThanOrEqualTo("date", DateTime.SpecifyKind(filterStartDate.Value, DateTimeKind.Utc));

            if (filterEndDate.HasValue)
                query = query.WhereLessThanOrEqualTo("date", DateTime.SpecifyKind(filterEndDate.Value, DateTimeKind.Utc)); 

            // Check the merchant
            if (!string.IsNullOrEmpty(filterMerchant))
                query = query.WhereEqualTo("merchant", filterMerchant);

            // Check the category
            if (!string.IsNullOrEmpty(filterCategory))
                query = query.WhereEqualTo("category", filterCategory);

            // Execute the query
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<TransactionModel> transactions = new List<TransactionModel>();
            foreach (DocumentSnapshot currentSnapshot in snapshot.Documents)
            {
                if (currentSnapshot.Exists)
                {
                    TransactionModel transaction = new TransactionModel
                    {
                        DocumentId = currentSnapshot.Id,
                        Amount = 0.0,
                        Date = DateTime.Now,
                        Merchant = string.Empty,
                        Category = string.Empty,
                        Username = string.Empty
                    };

                    if (currentSnapshot.TryGetValue<double>("amount", out var amountDouble))
                        transaction.Amount = amountDouble;
                    else if (currentSnapshot.TryGetValue<long>("amount", out var amountLong))
                        transaction.Amount = (double)amountLong;
                    else
                    {
                        transaction.Amount = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'amount' in transaction document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<DateTime>("date", out var date))
                        transaction.Date = date;
                    else
                    {
                        transaction.Date = DateTime.Now;
                        _logger.LogWarning($"Unexpected data type for 'date' in transaction document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<string>("merchant", out var merchant))
                        transaction.Merchant = merchant;
                    else
                    {
                        transaction.Merchant = string.Empty;
                        _logger.LogWarning($"Unexpected data type for 'merchant' in transaction document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<string>("category", out var category))
                        transaction.Category = category;
                    else
                    {
                        transaction.Category = string.Empty;
                        _logger.LogWarning($"Unexpected data type for 'category' in transaction document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<string>("username", out var username))
                        transaction.Username = username;
                    else
                    {
                        transaction.Username = string.Empty;
                        _logger.LogWarning($"Unexpected data type for 'username' in transaction document {currentSnapshot.Id}");
                    }

                    transactions.Add(transaction);
                }
            }

            // Sort the transactions list by the Date property in descending order
            transactions = transactions.OrderByDescending(t => t.Date).ToList();

            // Retrieve the asset information for the current user
            AssetModel asset = await _assetService.GetAssetByUsernameAsync(currentUser);

            // Create the ViewModel and populate it
            var viewModel = new TransactionsAndAssetsViewModel
            {
                Transactions = transactions,
                Asset = asset
            };


            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                _logger.LogError("Transaction ID is null or empty.");
                return RedirectToAction("Transactions");
            }

            try
            {
                DocumentReference transactionRef = _firestoreDb.Collection("Transactions").Document(transactionId);
                await transactionRef.DeleteAsync();
                TempData["SuccessMessage"] = "Transaction deleted successfully."; 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting transaction {transactionId}: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting transaction. Please try again."; 
            }

            return RedirectToAction("Transactions");
        }

        /// <summary>
        /// Adds a Transaction to the database and displays the Transaction View
        /// </summary>
        /// <param name="model">The new Transaction data</param>
        /// <returns>The Transaction View</returns>
        [HttpPost]
        public async Task<IActionResult> AddTransaction(TransactionModel model)
        {
            // Determine who is currently logged in
            string currentUser = HttpContext.Session.GetString("Username");

            if (currentUser == null || currentUser == "")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model is NOT valid. Here are the errors:");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    if (modelStateVal.Errors.Count > 0)
                    {
                        Console.WriteLine($"Key: {modelStateKey}, Errors:");
                        foreach (var error in modelStateVal.Errors)
                        {
                            Console.WriteLine($"- {error.ErrorMessage}");
                        }
                    }
                }
                return View(model); 
            }
            else
            {
                Console.WriteLine("Model is valid");
                model.Username = currentUser;

                CollectionReference transactionsRef = _firestoreDb.Collection(_transactionsCollection);

                // Keep the user-given date but set the time to now
                DateTime dt = DateTime.Now;
                model.Date = model.Date.Date + dt.TimeOfDay;

                Dictionary<string, object> newTransactionDocument = new Dictionary<string, object>
                {
                    { "amount", model.Amount },
                    { "date", DateTime.SpecifyKind(model.Date, DateTimeKind.Utc) },
                    { "merchant", model.Merchant},
                    { "category", model.Category },
                    { "username", currentUser }
                };

                await transactionsRef.AddAsync(newTransactionDocument);

                QuerySnapshot assetSnapshot = await _firestoreDb.Collection(_assetsCollection)
                    .WhereEqualTo("username", currentUser)
                    .Limit(1)
                    .GetSnapshotAsync();

                var assetDocument = assetSnapshot.Documents.FirstOrDefault();

                if (assetDocument != null)
                {
                    assetDocument.TryGetValue<double>("balance", out var currentBalance);
                    double newBalance = currentBalance - model.Amount;
                    DocumentReference assetRef = _firestoreDb.Collection(_assetsCollection).Document(assetDocument.Id);
                    Dictionary<string, object> updateBalance = new Dictionary<string, object>
                    {
                        { "balance", newBalance }
                    };
                    await assetRef.UpdateAsync(updateBalance);
                }

                return RedirectToAction("Transactions");
            }
        }

        [HttpGet]
        public async Task<IActionResult> YearlyReport()
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Get the Asset documenet for the current user
            QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            List<AssetModel> assets = new List<AssetModel>();

            // Go through each Asset document (there should only be one)
            foreach (DocumentSnapshot currentSnapshot in snapshot.Documents)
            {
                if (currentSnapshot.Exists)
                {
                    AssetModel asset = new AssetModel();

                    /*** Firestore can either store numbers as type long or double, so we need to make sure that we cast accordingly ***/

                    // Handle each numerical field with type checking
                    if (currentSnapshot.TryGetValue<double>("balance", out var balanceDouble))
                        asset.Balance = (double)balanceDouble;
                    else if (currentSnapshot.TryGetValue<long>("balance", out var balanceLong))
                        asset.Balance = (double)balanceLong;
                    else
                    {
                        // Handle the case where the field is missing or of an unexpected type
                        asset.Balance = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'balance' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("income", out var incomeDouble))
                        asset.Income = incomeDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("income", out var incomeLong))
                        asset.Income = (double)incomeLong * 12;
                    else
                    {
                        asset.Income = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'income' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("savings", out var savingsDouble))
                        asset.Savings = savingsDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("savings", out var savingsLong))
                        asset.Savings = (double)savingsLong * 12;
                    else
                    {
                        asset.Savings = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'savings' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("rent", out var rentDouble))
                        asset.Rent = rentDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("rent", out var rentLong))
                        asset.Rent = (double)rentLong * 12;
                    else
                    {
                        asset.Rent = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'rent' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("utilities", out var utilitiesDouble))
                        asset.Utilities = utilitiesDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("utilities", out var utilitiesLong))
                        asset.Utilities = (double)utilitiesLong * 12;
                    else
                    {
                        asset.Utilities = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'utilities' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("carPayment", out var carPaymentDouble))
                        asset.CarPayment = carPaymentDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("carPayment", out var carPaymentLong))
                        asset.CarPayment = (double)carPaymentLong * 12;
                    else
                    {
                        asset.CarPayment = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'carPayment' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("insurances", out var insurancesDouble))
                        asset.Insurances = insurancesDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("insurances", out var insurancesLong))
                        asset.Insurances = (double)insurancesLong * 12;
                    else
                    {
                        asset.Insurances = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'insurances' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("groceries", out var groceriesDouble))
                        asset.Groceries = groceriesDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("groceries", out var groceriesLong))
                        asset.Groceries = (double)groceriesLong * 12;
                    else
                    {
                        asset.Groceries = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'groceries' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("gas", out var gasDouble))
                        asset.Gas = gasDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("gas", out var gasLong))
                        asset.Gas = (double)gasLong * 12;
                    else
                    {
                        asset.Gas = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'gas' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("subscriptions", out var subscriptionsDouble))
                        asset.Subscriptions = subscriptionsDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("subscriptions", out var subscriptionsLong))
                        asset.Subscriptions = (double)subscriptionsLong * 12;
                    else
                    {
                        asset.Subscriptions = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'subscriptions' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<double>("other", out var otherDouble))
                        asset.Other = otherDouble * 12;
                    else if (currentSnapshot.TryGetValue<long>("other", out var otherLong))
                        asset.Other = (double)otherLong * 12;
                    else
                    {
                        asset.Other = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'other' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<string>("username", out var username))
                        asset.Username = username;
                    else
                    {
                        asset.Username = string.Empty;
                        _logger.LogWarning($"Unexpected data type for 'username' in document {currentSnapshot.Id}");
                    }

                    // If everything is good, add the asset to the list
                    assets.Add(asset);
                }
            }

            return View(assets);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Get the User document for the current user
            QuerySnapshot snapshot = await _firestoreDb.Collection(_usersCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            DocumentSnapshot userDocument = snapshot.Documents.First();

            // Check if the given current password matches the users actual current password
            string password = userDocument.GetValue<string>("password");

            if (!password.Equals(currentPassword))
            {
                // The password does not match
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, password))
            {
                // Current passwords do not match
                ViewBag.ErrorMessage = "The current password you entered is incorrect.";
                return View("Settings");
            }

            // Check if new password and confirm password match
            if (!newPassword.Equals(confirmPassword))
            {
                // New passwords do not match
                ViewBag.ErrorMessage = "The new password and confirm password do not match.";
                return View("Settings");
            }

            // Update the password
            DocumentReference assetRef = _firestoreDb.Collection(_usersCollection).Document(userDocument.Id);
            Dictionary<string, object> updatePassword = new Dictionary<string, object>
            {
                { "password", BCrypt.Net.BCrypt.HashPassword(newPassword) }
            };
            await assetRef.UpdateAsync(updatePassword);

            ViewBag.SuccessMessage = "Your password has been changed successfully!";
            return View("Settings");
        }
    }
}