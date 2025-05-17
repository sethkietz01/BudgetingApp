namespace BudgetingApp.Models
{
    public class TransactionsAndAssetsViewModel
    {
        public IEnumerable<TransactionModel> Transactions { get; set; }
        public AssetModel Asset { get; set; }
    }
}
