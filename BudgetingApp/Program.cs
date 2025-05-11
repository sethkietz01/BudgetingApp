using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using System.Text.Json; // Required for deserializing JSON from environment variable

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Session Services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Initialize Firestore using environment variable for credentials
string projectId = "budgetingdatabase";
string firebaseCredentialsJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");

try
{
    if (!string.IsNullOrEmpty(firebaseCredentialsJson))
    {
        GoogleCredential credential = GoogleCredential.FromJson(firebaseCredentialsJson);
        FirestoreDbBuilder dbBuilder = new FirestoreDbBuilder
        {
            ProjectId = projectId,
            Credential = credential
        };
        FirestoreDb db = dbBuilder.Build();
        builder.Services.AddSingleton(db); // Register FirestoreDb as a singleton
        Console.WriteLine("Firestore initialized successfully using environment variable.");
    }
    else
    {
        // Fallback to file-based configuration if the environment variable is not set
        string credentialsPath = Path.Combine(AppContext.BaseDirectory, "Config", "budgetingdatabase-firebase-adminsdk-fbsvc-d6fd0e7d46.json");
        if (File.Exists(credentialsPath))
        {
            GoogleCredential credential = GoogleCredential.FromFile(credentialsPath);
            FirestoreDbBuilder dbBuilder = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                Credential = credential
            };
            FirestoreDb db = dbBuilder.Build();
            builder.Services.AddSingleton(db); // Register FirestoreDb as a singleton
            Console.WriteLine("Firestore initialized successfully using file-based configuration.");
        }
        else
        {
            Console.WriteLine("Warning: FIREBASE_SERVICE_ACCOUNT_JSON environment variable not set and Firebase credentials file not found.");
        }
    }
}
catch (Exception ex)
{
    // Log the error message - crucial for debugging in Azure
    Console.WriteLine($"Error initializing Firestore: {ex.Message}");
}

builder.Services.AddScoped<UserService>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Session Middleware
app.UseSession();

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();  // Enable authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();