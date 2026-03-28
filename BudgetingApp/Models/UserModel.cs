using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace BudgetingApp.Models
{
    public class UserModel
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        [FirestoreProperty("mfaEnabled")]
        public bool? MfaEnabled { get; set; }

        [FirestoreProperty("mfaSecretKey")]
        public string? MfaSecretKey { get; set; }
    }
}
