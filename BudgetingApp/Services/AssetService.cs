using BudgetingApp.Models;
using Google.Cloud.Firestore;

namespace BudgetingApp.Services
{
    public class AssetService
    {
        private readonly FirestoreDb _db;
        private readonly ILogger<FirestoreAssetService> _logger;
        private readonly string _collection = "Assets";

        public AssetService(FirestoreDb db, ILogger<FirestoreAssetService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public AssetModel MapSnapshotToAsset(DocumentSnapshot snapshot)
        {
                if (!snapshot.Exists) return null;

                /*** Firestore can either store numbers as type long or double, so we need to make sure that we cast accordingly ***/

                var asset = new AssetModel
                {
                    Balance = GetNumericValue(snapshot, "balance"),
                    Income = GetNumericValue(snapshot, "income"),
                    Savings = GetNumericValue(snapshot, "savings"),
                    Rent = GetNumericValue(snapshot, "rent"),
                    Utilities = GetNumericValue(snapshot, "utilities"),
                    CarPayment = GetNumericValue(snapshot, "carPayment"),
                    Insurances = GetNumericValue(snapshot, "insurances"),
                    Groceries = GetNumericValue(snapshot, "groceries"),
                    Gas = GetNumericValue(snapshot, "gas"),
                    Subscriptions = GetNumericValue(snapshot, "subscriptions"),
                    Other = GetNumericValue(snapshot, "other"),
                    Username = snapshot.TryGetValue<string>("username", out var username)
                               ? username
                               : string.Empty
                };

                if (string.IsNullOrEmpty(asset.Username))
                    _logger.LogWarning($"Missing or invalid username in document {snapshot.Id}");

            return asset;
        }

        public List<AssetModel> MapSnapshotsToAssets(QuerySnapshot snapshot)
        {
            var assets = new List<AssetModel>();
            foreach (var doc in snapshot.Documents)
            {
                var asset = MapSnapshotToAsset(doc);
                if (asset != null) assets.Add(asset);
            }
            return assets;
        }

        private double GetNumericValue(DocumentSnapshot snapshot, string field)
        {
            if (snapshot.TryGetValue<double>(field, out var dblValue))
                return dblValue;

            if (snapshot.TryGetValue<long>(field, out var lngValue))
                return (double)lngValue;

            _logger.LogWarning($"Unexpected data type or missing field for '{field}' in document {snapshot.Id}");
            return 0.0;
        }
    }
}
