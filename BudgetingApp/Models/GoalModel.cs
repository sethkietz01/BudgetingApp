using Google.Cloud.Firestore;

namespace BudgetingApp.Models
{
    public class GoalModel
    {
        [FirestoreProperty("goalName")]
        public string GoalName { get; set; }

        [FirestoreProperty("goalPriority")]
        public int GoalPriority { get; set; }

        [FirestoreProperty("goalAmount")]
        public double GoalAmount { get; set; }

        [FirestoreProperty("savedAmount")]
        public double SavedAmount { get; set; }

        [FirestoreProperty("goalDate")]
        public DateTime GoalDate { get; set; }

        [FirestoreProperty("username")]
        public string Username { get; set; }
    }
}
