using BudgetingApp.Models;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BudgetingApp.Services
{
    public class FirestoreAssetService : IAssetService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly ILogger<FirestoreAssetService> _logger;
        private readonly string _assetCollectionName = "Assets"; 

        public FirestoreAssetService(FirestoreDb firestoreDb, ILogger<FirestoreAssetService> logger)
        {
            _firestoreDb = firestoreDb;
            _logger = logger;
        }

        public async Task<AssetModel> GetAssetByUsernameAsync(string username)
        {
            try
            {
                QuerySnapshot snapshot = await _firestoreDb.Collection(_assetCollectionName)
                    .WhereEqualTo("username", username)
                    .Limit(1) 
                    .GetSnapshotAsync();

                if (snapshot.Documents.Count > 0)
                {
                    DocumentSnapshot document = snapshot.Documents.First();
                    if (document.Exists)
                    {
                        AssetModel asset = new AssetModel();

                        /*** Firestore can either store numbers as type long or double, so we need to make sure that we cast accordingly ***/

                        // Handle each numerical field with type checking
                        if (document.TryGetValue<double>("balance", out var balanceDouble))
                            asset.Balance = (double)balanceDouble;
                        else if (document.TryGetValue<long>("balance", out var balanceLong))
                            asset.Balance = (double)balanceLong;
                        else
                        {
                            // Handle the case where the field is missing or of an unexpected type
                            asset.Balance = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'balance' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("income", out var incomeDouble))
                            asset.Income = incomeDouble;
                        else if (document.TryGetValue<long>("income", out var incomeLong))
                            asset.Income = (double)incomeLong;
                        else
                        {
                            asset.Income = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'income' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("savings", out var savingsDouble))
                            asset.Savings = savingsDouble;
                        else if (document.TryGetValue<long>("savings", out var savingsLong))
                            asset.Savings = (double)savingsLong;
                        else
                        {
                            asset.Savings = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'savings' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("rent", out var rentDouble))
                            asset.Rent = rentDouble;
                        else if (document.TryGetValue<long>("rent", out var rentLong))
                            asset.Rent = (double)rentLong;
                        else
                        {
                            asset.Rent = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'rent' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("utilities", out var utilitiesDouble))
                            asset.Utilities = utilitiesDouble;
                        else if (document.TryGetValue<long>("utilities", out var utilitiesLong))
                            asset.Utilities = (double)utilitiesLong;
                        else
                        {
                            asset.Utilities = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'utilities' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("carPayment", out var carPaymentDouble))
                            asset.CarPayment = carPaymentDouble;
                        else if (document.TryGetValue<long>("carPayment", out var carPaymentLong))
                            asset.CarPayment = (double)carPaymentLong;
                        else
                        {
                            asset.CarPayment = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'carPayment' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("insurances", out var insurancesDouble))
                            asset.Insurances = insurancesDouble;
                        else if (document.TryGetValue<long>("insurances", out var insurancesLong))
                            asset.Insurances = (double)insurancesLong;
                        else
                        {
                            asset.Insurances = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'insurances' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("groceries", out var groceriesDouble))
                            asset.Groceries = groceriesDouble;
                        else if (document.TryGetValue<long>("groceries", out var groceriesLong))
                            asset.Groceries = (double)groceriesLong;
                        else
                        {
                            asset.Groceries = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'groceries' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("gas", out var gasDouble))
                            asset.Gas = gasDouble;
                        else if (document.TryGetValue<long>("gas", out var gasLong))
                            asset.Gas = (double)gasLong;
                        else
                        {
                            asset.Gas = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'gas' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("subscriptions", out var subscriptionsDouble))
                            asset.Subscriptions = subscriptionsDouble;
                        else if (document.TryGetValue<long>("subscriptions", out var subscriptionsLong))
                            asset.Subscriptions = (double)subscriptionsLong;
                        else
                        {
                            asset.Subscriptions = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'subscriptions' in document {document.Id}");
                        }

                        if (document.TryGetValue<double>("other", out var otherDouble))
                            asset.Other = otherDouble;
                        else if (document.TryGetValue<long>("other", out var otherLong))
                            asset.Other = (double)otherLong;
                        else
                        {
                            asset.Other = 0.0;
                            _logger.LogWarning($"Unexpected data type for 'other' in document {document.Id}");
                        }


                        // If everything is good, add the asset to the list
                        asset.Username = document.GetValue<string>("username"); 
                        return asset;
                    }
                    else
                    {
                        _logger.LogWarning($"Asset document not found for username: {username}");
                        return null;
                    }
                }
                else
                {
                    _logger.LogInformation($"No asset found for username: {username}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving asset for username {username}: {ex.Message}");
                return null;
            }
        }
    }
}