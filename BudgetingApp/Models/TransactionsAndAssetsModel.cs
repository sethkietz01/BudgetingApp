namespace BudgetingApp.Models
{
    public class TransactionsAndAssetsViewModel
    {
        public IEnumerable<TransactionModel> Transactions { get; set; }
        public ExpenseModel Asset { get; set; }
    }
}
