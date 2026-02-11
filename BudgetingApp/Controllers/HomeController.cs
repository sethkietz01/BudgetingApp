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
        private readonly string _goalsCollection = "Goals";
        private readonly BudgetingApp.Services.IAssetService _iassetService;
        private readonly AssetService _assetService;


        public HomeController(ILogger<HomeController> logger, FirestoreDb firestoreDb, IAssetService iassetService, AssetService assetService)
        {
            _logger = logger;
            _firestoreDb = firestoreDb;
            _iassetService = iassetService;
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
            List<AssetModel> assets = await _assetService.GetAssetsByUsernameAsync(currentUser);

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

            AssetModel asset = await _assetService.GetAssetByUsernameAsync(currentUser);

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
            // Determine who is logged in
            string currentUser = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid) return View(model);

            var updates = new Dictionary<string, object>();

            // Helper to parse form and add to dictionary
            void TryAddUpdate(string formKey, string dbKey)
            {
                if (!string.IsNullOrEmpty(Request.Form[formKey]))
                {
                    if (double.TryParse(Request.Form[formKey], out double val))
                        updates[dbKey] = val;
                    else
                        ModelState.AddModelError(formKey, $"The {formKey} must be a valid number.");
                }
            }

            // Map fields
            TryAddUpdate("Balance", "balance");
            TryAddUpdate("Income", "income");
            TryAddUpdate("Savings", "savings");
            TryAddUpdate("Rent", "rent");
            TryAddUpdate("Utilities", "utilities");
            TryAddUpdate("CarPayment", "carPayment");
            TryAddUpdate("Insurances", "insurances");
            TryAddUpdate("Groceries", "groceries");
            TryAddUpdate("Gas", "gas");
            TryAddUpdate("Subscriptions", "subscriptions");
            TryAddUpdate("Other", "other");

            if (!ModelState.IsValid) return View(model);

            bool success = await _assetService.UpdateAssetsAsync(currentUser, updates);

            if (!success) return NotFound();

            return RedirectToAction("Index");
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
            
            bool success = await _assetService.IncrementBalanceAsync(currentUser, amountToAdd);

            if (!success)
                return NotFound($"No asset document found for user: {currentUser}");

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

            bool success = await _assetService.IncrementBalanceAsync(currentUser, amountToSubtract);
            
            if (!success)   
                return NotFound($"No asset document found for user: {currentUser}");

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

            List<AssetModel> assets = _assetService.MapSnapshotsToAssets(snapshot);

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
            AssetModel asset = await _iassetService.GetAssetByUsernameAsync(currentUser);

            // Create the ViewModel and populate it
            var viewModel = new TransactionsAndAssetsViewModel
            {
                Transactions = transactions,
                Asset = asset
            };


            return View(viewModel);
        }

        /// <summary>
        /// Deletes a transaction from the database
        /// </summary>
        /// <param name="transactionId">The ID for the transaction to delete</param>
        /// <returns>The Transactions View</returns>
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
        /// <returns>The Transactions View</returns>
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

        /// <summary>
        /// Changes the password of the current user
        /// </summary>
        /// <param name="currentPassword">The user's current password</param>
        /// <param name="newPassword">The new password</param>
        /// <param name="confirmPassword">A confirmation of the new password</param>
        /// <returns>The Settings view</returns>
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

        /// <summary>
        /// Populates and returns the Goals page with all of the current user's goals
        /// </summary>
        /// <returns>The Goals view</returns>
        [HttpGet]
        public async Task<IActionResult> Goals()
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Get the Asset documenet for the current user
            QuerySnapshot snapshot = await _firestoreDb.Collection(_goalsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();


            List<GoalModel> goals = new List<GoalModel>();
            foreach (DocumentSnapshot currentSnapshot in snapshot.Documents)
            {
                if (currentSnapshot.Exists)
                {
                    GoalModel goal = new GoalModel
                    {
                        DocumentId = currentSnapshot.Id,
                        GoalName = string.Empty,
                        GoalPriority = 0,
                        GoalAmount = 0,
                        SavedAmount = 0,
                        GoalDate = DateTime.Now,
                        Username = currentUser
                    };

                    if (currentSnapshot.TryGetValue<string>("goalName", out var goalName))
                        goal.GoalName = goalName;
                    else
                        _logger.LogWarning($"Unexpected data type for 'goalName' in goal document {currentSnapshot.Id}");

                    if (currentSnapshot.TryGetValue<int>("goalPriority", out var goalPriority))
                        goal.GoalPriority = goalPriority;
                    else
                        _logger.LogWarning($"Unexpected data type for 'goalPriority' in goal document {currentSnapshot.Id}");

                    if (currentSnapshot.TryGetValue<double>("goalAomunt", out var goalAmountDouble))
                        goal.GoalAmount = goalAmountDouble;
                    else if (currentSnapshot.TryGetValue<long>("goalAmount", out var goalAmountLong))
                        goal.GoalAmount = goalAmountLong;
                    else
                        _logger.LogWarning($"Unexpected data type for 'goalAmount' in goal document {currentSnapshot.Id}");

                    if (currentSnapshot.TryGetValue<double>("savedAmount", out var savedAmountDouble))
                        goal.SavedAmount = savedAmountDouble;
                    else if (currentSnapshot.TryGetValue<long>("savedAmount", out var savedAmountLong))
                        goal.SavedAmount = savedAmountLong;
                    else
                        _logger.LogWarning($"Unexpected data type for 'savedAmount' in goal document {currentSnapshot.Id}");

                    if (currentSnapshot.TryGetValue<DateTime>("goalDate", out var goalDate))
                        goal.GoalDate = goalDate;
                    else
                        _logger.LogWarning($"Unexpected data type for 'goalDate' in goal document {currentSnapshot.Id}");

                    if (currentSnapshot.TryGetValue<string>("username", out var username))
                        goal.Username = username;
                    else
                        _logger.LogWarning($"Unexpected data type for 'username' in goal document {currentSnapshot.Id}");


                    goals.Add(goal);
                }
            }

            // Sort the goals list by the priority in descending order
            goals = goals.OrderByDescending(g => g.GoalPriority).ToList();

            return View(goals);
        }


        /// <summary>
        /// Deletes a goal from the database
        /// </summary>
        /// <param name="goalId">The ID for the goal to delete</param>
        /// <returns>The Goals view</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteGoal(string goalId)
        {
            if (string.IsNullOrEmpty(goalId))
            {
                _logger.LogError("Goal ID is null or empty.");
                return RedirectToAction("Goals");
            }

            try
            {
                DocumentReference transactionRef = _firestoreDb.Collection("Goals").Document(goalId);
                await transactionRef.DeleteAsync();
                TempData["SuccessMessage"] = "Goal deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting goal {goalId}: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting goal. Please try again.";
            }

            return RedirectToAction("Goals");
        }

        /// <summary>
        /// Gets the information of the goal to edit
        /// </summary>
        /// <param name="goalId">The ID of the goal to edit</param>
        /// <returns>The Edit Goal view</returns>
        [HttpGet]
        public async Task<IActionResult> EditGoal(string goalId)
        {
            // Determine who is currently logged in 
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Check if the goalId is valid
            if (string.IsNullOrEmpty(goalId))
            {
                _logger.LogError("Goal ID is null or empty.");
                return RedirectToAction("Goals");
            }

            // Get the goal document from the Firestore
            DocumentReference goalReference = _firestoreDb.Collection("Goals").Document(goalId);
            DocumentSnapshot goalSnapshot = await goalReference.GetSnapshotAsync();

            Dictionary<string, object> goalDocument = goalSnapshot.ToDictionary();

            goalSnapshot.TryGetValue<DateTime>("goalDate", out var date);

            // Create the goal model
            GoalModel goal = new GoalModel
            {
                DocumentId = goalId,
                Username = currentUser,
                GoalName = goalDocument["goalName"].ToString(),
                GoalPriority = Convert.ToInt32(goalDocument["goalPriority"]),
                GoalAmount = Convert.ToDouble(goalDocument["goalAmount"]),
                SavedAmount = Convert.ToDouble(goalDocument["savedAmount"]),
                GoalDate = date
            };

            return View(goal);
        }

        /// <summary>
        /// Updates a goal in the database
        /// </summary>
        /// <param name="model">The goal data</param>
        /// <returns>The Goals view</returns>
        [HttpPost]
        public async Task<IActionResult> EditGoal(GoalModel model)
        {
            // Determine who is currently logged in
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                DocumentReference goalReference = _firestoreDb.Collection("Goals").Document(model.DocumentId);
                DocumentSnapshot goalSnapshot = await goalReference.GetSnapshotAsync();

                Dictionary<string, object> goalDocument = goalSnapshot.ToDictionary();

                goalSnapshot.TryGetValue<DateTime>("goalDate", out var date);

                if (!goalSnapshot.Exists)
                {
                    // Handle case where the document no longer exists
                    ModelState.AddModelError("", "The goal you are trying to edit does not exist.");
                    return View("Goals", model);
                }

                GoalModel existingGoal = new GoalModel
                {
                    GoalName = goalDocument["goalName"].ToString(),
                    GoalPriority = Convert.ToInt32(goalDocument["goalPriority"]),
                    GoalAmount = Convert.ToDouble(goalDocument["goalAmount"]),
                    SavedAmount = Convert.ToDouble(goalDocument["savedAmount"]),
                    GoalDate = DateTime.SpecifyKind(date, DateTimeKind.Utc)

                };


                Dictionary<string, object> updates = new Dictionary<string, object>();

                // Compare and add to updates only if the value has changed or new input is provided
                if (!string.IsNullOrEmpty(model.GoalName) && model.GoalName != existingGoal.GoalName)
                    updates.Add("goalName", model.GoalName);

                if (model.GoalAmount != existingGoal.GoalAmount && model.GoalAmount != 0)
                    updates.Add("goalAmount", model.GoalAmount);

                if (model.GoalDate != default(DateTime) && model.GoalDate.Date != existingGoal.GoalDate.Date)
                    updates.Add("goalDate", DateTime.SpecifyKind(model.GoalDate, DateTimeKind.Utc));

                if (model.GoalPriority != existingGoal.GoalPriority && model.GoalPriority >= 0 && model.GoalPriority <= 2)
                    updates.Add("goalPriority", model.GoalPriority);

                if (model.SavedAmount != existingGoal.SavedAmount && model.SavedAmount != 0)
                    updates.Add("savedAmount", model.SavedAmount);

                if (updates.Count > 0)
                    await goalReference.UpdateAsync(updates);
                else
                    Console.WriteLine("No changes detected, not updating Firestore.");

                return RedirectToAction("Goals"); 
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

            return View("Goals"); 
        }

        /// <summary>
        /// Displays the Add Goal view
        /// </summary>
        /// <returns>The Add Goal view</returns>
        [HttpGet]
        public IActionResult AddGoal()
        {
            // Determine who is currently logged in
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            return View();
        }

        /// <summary>
        /// Adds a goal to the database
        /// </summary>
        /// <param name="model">The new goal data</param>
        /// <returns>The Goals view</returns>
        [HttpPost]
        public async Task<IActionResult> AddGoal(GoalModel model)
        {
            // Determine who is currently logged in
            string currentUser = HttpContext.Session.GetString("Username");

            // Redirect to the login page if the user is not signed in
            if (string.IsNullOrEmpty(currentUser))
                return RedirectToAction("Login", "Auth");

            // Convert the new goal to a dictionary
            Dictionary<string, object> goal = new Dictionary<string, object>
            {
                { "username", currentUser },
                { "goalName", model.GoalName },
                { "goalAmount", model.GoalAmount },
                { "goalPriority", model.GoalPriority },
                { "goalDate", DateTime.SpecifyKind(model.GoalDate, DateTimeKind.Utc) },
                { "savedAmount", model.SavedAmount }
            };

            // Add the goal to the Firestore
            DocumentReference addedGoal = await _firestoreDb.Collection(_goalsCollection).AddAsync(goal);

            return RedirectToAction("Goals");
        }
    }
}