using BudgetingApp.Models;
using Google.Cloud.Firestore;
using System.Threading.Tasks;

public class UserService
{
    private readonly FirestoreDb _firestoreDb;
    private readonly string _usersCollection = "Users";

    public UserService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<UserModel> AuthenticateUserAsync(string username, string password)
    {
        QuerySnapshot snapshot = await _firestoreDb.Collection(_usersCollection)
            .WhereEqualTo(nameof(UserModel.Username), username)
            .WhereEqualTo(nameof(UserModel.Password), password) 
            .Limit(1)
            .GetSnapshotAsync();

        if (snapshot.Documents.Count > 0)
        {
            return snapshot.Documents[0].ConvertTo<UserModel>();
        }

        return null;
    }

    // ... other methods (registration, etc.)
}