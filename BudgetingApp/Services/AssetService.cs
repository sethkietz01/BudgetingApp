using BudgetingApp.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace BudgetingApp.Services
{
    public class AssetService : IAssetService
    {
        private readonly FirestoreDb _db;
        private readonly string _assetsCollection = "Assets";

        public AssetService(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<QuerySnapshot> GetSnapshotByUsernameAsync(string currentUser)
        {
            QuerySnapshot snapshot = await _db.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            return snapshot;
        }

        public async Task<AssetModel> GetAssetByUsernameAsync(string currentUser)
        {
            QuerySnapshot snapshot = await _db.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            AssetModel asset = MapSnapshotToAsset(snapshot.Documents[0]);

            return asset;
        }

        public async Task<List<AssetModel>> GetAssetsByUsernameAsync(string currentUser)
        {
            QuerySnapshot snapshot = await _db.Collection(_assetsCollection)
                .WhereEqualTo("username", currentUser)
                .GetSnapshotAsync();

            List<AssetModel> assets = MapSnapshotsToAssets(snapshot);

            return assets;
        }

        public AssetModel MapSnapshotToAsset(DocumentSnapshot snapshot)
        {
                if (!snapshot.Exists) return null;

                /*** Firestore can either store numbers as type long or double, so we need to make sure that we cast accordingly ***/

                var asset = new AssetModel
                {
                    Balance = Math.Round(GetNumericValue(snapshot, "balance"), 2),
                    Income = Math.Round(GetNumericValue(snapshot, "income"), 2),
                    Savings = Math.Round(GetNumericValue(snapshot, "savings"), 2),
                    Rent = Math.Round(GetNumericValue(snapshot, "rent"), 2),
                    Utilities = Math.Round(GetNumericValue(snapshot, "utilities"), 2),
                    CarPayment = Math.Round(GetNumericValue(snapshot, "carPayment"), 2),
                    Insurances = Math.Round(GetNumericValue(snapshot, "insurances"), 2),
                    Groceries = Math.Round(GetNumericValue(snapshot, "groceries"), 2),
                    Gas = Math.Round(GetNumericValue(snapshot, "gas"), 2),
                    Subscriptions = Math.Round(GetNumericValue(snapshot, "subscriptions"), 2),
                    Other = Math.Round(GetNumericValue(snapshot, "other"), 2),
                    Username = snapshot.TryGetValue<string>("username", out var username)
                               ? username
                               : string.Empty
                };

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

            return 0.0;
        }

        public async Task<bool> UpdateAssetsAsync(string username, Dictionary<string, object> updates)
        {
            if (updates == null || updates.Count == 0) return true;

            QuerySnapshot snapshot = await GetSnapshotByUsernameAsync(username);
            DocumentSnapshot assetDocument = snapshot.Documents.FirstOrDefault();

            if (assetDocument == null) return false;

            DocumentReference assetRef = _db.Collection(_assetsCollection).Document(assetDocument.Id);

            await assetRef.UpdateAsync(updates);
            return true;
        }

        public async Task<bool> IncrementBalanceAsync(string username, double amount)
        {
            QuerySnapshot snapshot = await GetSnapshotByUsernameAsync(username);
            DocumentSnapshot assetDocument = snapshot.Documents.FirstOrDefault();

            if (assetDocument == null) return false;

            DocumentReference assetRef = _db.Collection(_assetsCollection).Document(assetDocument.Id);

            await assetRef.UpdateAsync("balance", FieldValue.Increment(amount));

            return true;
        }

        public async Task<bool> DeleteData(string currentUser, FirestoreDb _firestoreDb, string _transactionsCollection, string _goalsCollection, string _assetsCollection)
        {
            // Attempt to delete data
            try
            {
                WriteBatch batch = _firestoreDb.StartBatch();

                // Get and delete all transactions
                Query transQuery = _firestoreDb.Collection(_transactionsCollection).WhereEqualTo("username", currentUser);
                QuerySnapshot transSnapshot = await transQuery.GetSnapshotAsync();
                foreach (DocumentSnapshot doc in transSnapshot.Documents)
                {
                    batch.Delete(doc.Reference);
                }

                // Get and delete all goals
                Query goalQuery = _firestoreDb.Collection(_goalsCollection).WhereEqualTo("username", currentUser);
                QuerySnapshot goalSnapshot = await goalQuery.GetSnapshotAsync();
                foreach (DocumentSnapshot doc in goalSnapshot.Documents)
                {
                    batch.Delete(doc.Reference);
                }

                Query assetQuery = _firestoreDb.Collection(_assetsCollection).WhereEqualTo("username", currentUser).Limit(1);
                QuerySnapshot assetSnapshot = await assetQuery.GetSnapshotAsync();

                var assetDoc = assetSnapshot.Documents.FirstOrDefault();
                if (assetDoc != null)
                {
                    // Set all expenses data back to 0
                    var resetValues = new Dictionary<string, object>
                    {
                        { "balance", 0.0 },
                        { "income", 0.0 },
                        { "savings", 0.0 },
                        { "rent", 0.0 },
                        { "utilities", 0.0 },
                        { "carPayment", 0.0 },
                        { "insurances", 0.0 },
                        { "groceries", 0.0 },
                        { "gas", 0.0 },
                        { "subscriptions", 0.0 },
                        { "other", 0.0 }
                    };
                    batch.Update(assetDoc.Reference, resetValues);
                }

                await batch.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
