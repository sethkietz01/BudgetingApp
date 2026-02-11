using BudgetingApp.Models;
using System.Threading.Tasks;

namespace BudgetingApp.Services
{
    public interface IAssetService
    {
        Task<AssetModel> GetAssetByUsernameAsync(string username);
        Task<bool> IncrementBalanceAsync(string username, double amount);
    }
}