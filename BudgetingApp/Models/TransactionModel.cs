using Google.Cloud.Firestore;

namespace BudgetingApp.Models
{
    public class TransactionModel
    {
        [FirestoreProperty("amount")]
        public double Amount { get; set; }

        [FirestoreProperty("date")]
        public DateTime Date { get; set; }

        [FirestoreProperty("merchant")]
        public string Merchant {  get; set; }

        [FirestoreProperty("username")]
        public string Username { get; set; }
    }
}
