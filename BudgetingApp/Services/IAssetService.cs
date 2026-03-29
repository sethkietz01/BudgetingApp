using BudgetingApp.Models;
using System.Threading.Tasks;

namespace BudgetingApp.Services
{
    public interface IAssetService
    {
        Task<ExpenseModel> GetAssetByUsernameAsync(string username);
        Task<bool> IncrementBalanceAsync(string username, double amount);
    }
}