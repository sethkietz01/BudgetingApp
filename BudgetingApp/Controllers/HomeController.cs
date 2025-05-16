using BudgetingApp.Models;
using Google.Apis.Logging;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BudgetingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirestoreDb _firestoreDb;
        private readonly string _assetsCollection = "Assets";



        public HomeController(ILogger<HomeController> logger, FirestoreDb firestoreDb)
        {
            _logger = logger;
            _firestoreDb = firestoreDb;
        }

        public async Task<IActionResult> Index()
        {
            string currentUser = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

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

                    if (currentSnapshot.TryGetValue<double>("rent", out var rentDouble))
                        asset.Rent = rentDouble;
                    else if (currentSnapshot.TryGetValue<long>("rent", out var rentLong))
                        asset.Rent = (double)rentLong;
                    else
                    {
                        asset.Rent = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'rent' in document {currentSnapshot.Id}");
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

            int count = assets.Count;

            return View(assets);
        }

        [HttpGet]
        public async Task<IActionResult> EditAssets()
        {
            string currentUser = HttpContext.Session.GetString("Username");

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

            if (currentSnapshot.TryGetValue<double>("rent", out var rentDouble))
                asset.Rent = rentDouble;
            else if (currentSnapshot.TryGetValue<long>("rent", out var rentLong))
                asset.Rent = (double)rentLong;
            else
            {
                asset.Rent = 0.0;
                _logger.LogWarning($"Unexpected data type for 'rent' in document {currentSnapshot.Id}");
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
            {
                return NotFound();
            }

            return View(asset);
        }

        [HttpPost]
        public async Task<IActionResult> EditAssets(AssetModel model)
        {
            string currentUser = HttpContext.Session.GetString("Username");

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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> AddToBalance(int amountToAdd)
        {

            string currentUser = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
                return NotFound($"No asset document found for user: {currentUser}");

            DocumentSnapshot assetDocument = snapshot.Documents.First();
            DocumentReference assetReference = _firestoreDb.Collection(_assetsCollection).Document(assetDocument.Id);

            await assetReference.UpdateAsync("balance", FieldValue.Increment(amountToAdd));

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SubtractFromBalance(int amountToSubtract)
        {
            // Make the amount negative
            amountToSubtract = amountToSubtract * (-1);

            Console.WriteLine(amountToSubtract);

            string currentUser = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            QuerySnapshot snapshot = await _firestoreDb.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            if (snapshot.Documents.Count == 0)
                return NotFound($"No asset document found for user: {currentUser}");

            DocumentSnapshot assetDocument = snapshot.Documents.First();
            DocumentReference assetReference = _firestoreDb.Collection(_assetsCollection).Document(assetDocument.Id);

            await assetReference.UpdateAsync("balance", FieldValue.Increment(amountToSubtract));

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> WhatIf()
        {
            string currentUser = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

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

                    if (currentSnapshot.TryGetValue<double>("rent", out var rentDouble))
                        asset.Rent = rentDouble;
                    else if (currentSnapshot.TryGetValue<long>("rent", out var rentLong))
                        asset.Rent = (double)rentLong;
                    else
                    {
                        asset.Rent = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'rent' in document {currentSnapshot.Id}");
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

            int count = assets.Count;

            return View(assets);
        }

        public IActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login", "Auth");
        }
    }
}