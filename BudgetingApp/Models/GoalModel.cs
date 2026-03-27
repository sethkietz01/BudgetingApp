    using Google.Cloud.Firestore;
    using System.ComponentModel.DataAnnotations;

    namespace BudgetingApp.Models
    {
        public class GoalModel
        {
            public string? DocumentId { get; set; }

            [FirestoreProperty("goalName")]
            [Required(ErrorMessage = "Goal name is required.")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Goal name must be between 2 and 50 characters.")]
            public string GoalName { get; set; }

            [FirestoreProperty("goalPriority")]
            [Required(ErrorMessage = "Goal priority is required.")]
            public int GoalPriority { get; set; }

            [FirestoreProperty("goalAmount")]
            [Required(ErrorMessage = "Goal amount is required.")]
            [Range(0.0, double.MaxValue, ErrorMessage = "Goal amount must be greater than 0.")]
            public double GoalAmount { get; set; }

            [FirestoreProperty("savedAmount")]
            [Required(ErrorMessage = "Saved amount is required.")]
            [Range(0.0, double.MaxValue, ErrorMessage = "Saved amount must be greater than 0.")]
            public double SavedAmount { get; set; }

            [FirestoreProperty("goalDate")]
            [Required(ErrorMessage = "Goal date is required.")]
            [DataType(DataType.Date)]
            public DateTime GoalDate { get; set; }

            [FirestoreProperty("username")]
            public string? Username { get; set; }
        }
    }
