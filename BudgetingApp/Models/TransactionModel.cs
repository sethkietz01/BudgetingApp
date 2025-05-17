using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace BudgetingApp.Models
{
    public class TransactionModel
    {
        public string DocumentId { get; set; }

        [FirestoreProperty("amount")]
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public double Amount { get; set; }

        [FirestoreProperty("date")]
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)] 
        public DateTime Date { get; set; }

        [FirestoreProperty("merchant")]
        [Required(ErrorMessage = "Merchant is required.")]
        public string Merchant { get; set; }

        [FirestoreProperty("Category")]
        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }

        [FirestoreProperty("username")]
        public string Username { get; set; }
    }
}