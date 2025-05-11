using BudgetingApp.Models;
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
                    if (currentSnapshot.TryGetValue<long>("balance", out var balanceLong))
                        asset.Balance = (double)balanceLong;
                    else if (currentSnapshot.TryGetValue<double>("balance", out var balanceDouble))
                        asset.Balance = balanceDouble;
                    else
                    {
                        // Handle the case where the field is missing or of an unexpected type
                        asset.Balance = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'balance' in document {currentSnapshot.Id}");
                    }

                    if (currentSnapshot.TryGetValue<long>("carPayment", out var carPaymentLong))
                        asset.CarPayment = (double)carPaymentLong;
                    else if (currentSnapshot.TryGetValue<double>("carPayment", out var carPaymentDouble))
                        asset.CarPayment = carPaymentDouble;
                    else
                    {
                        asset.CarPayment = 0.0;
                        _logger.LogWarning($"Unexpected data type for 'carPayment' in document {currentSnapshot.Id}");
                    }

                    // Repeat the above pattern for other numerical fields (gas, groceries, income, rent, subscriptions)
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

        public IActionResult EditAssets()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
