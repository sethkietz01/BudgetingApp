using Google.Cloud.Firestore;

namespace BudgetingApp.Models
{
    public class AssetModel
    {
        [FirestoreProperty("balance")]
        public double Balance { get; set; }

        [FirestoreProperty("carPayment")]
        public double CarPayment { get; set; }

        [FirestoreProperty("gas")]
        public double Gas { get; set; }

        [FirestoreProperty("groceries")]
        public double Groceries { get; set; }

        [FirestoreProperty("income")]
        public double Income { get; set; }

        [FirestoreProperty("rent")]
        public double Rent { get; set; }

        [FirestoreProperty("utilities")]
        public double Utilities { get; set; }

        [FirestoreProperty("insurances")]
        public double Insurances { get; set; }

        [FirestoreProperty("subscriptions")]
        public double Subscriptions { get; set; }

        [FirestoreProperty("other")]
        public double Other {  get; set; }

        [FirestoreProperty("username")]
        public string Username { get; set; }
    }
}