// System.Text.Json is not strictly needed for this approach, but keep if used elsewhere
// using System.Text.Json; 
using BudgetingApp.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System.Text.Json;

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

string credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

try
{
    string jsonCredentialsString = File.ReadAllText(credentialsPath);
    Console.WriteLine("Successfully read GOOGLE_APPLICATION_CREDENTIALS file content.");

    string projectId = "";

    using (JsonDocument document = JsonDocument.Parse(jsonCredentialsString))
    {
        JsonElement root = document.RootElement;
        if (root.TryGetProperty("project_id", out JsonElement projectIdElement))
        {
            projectId = projectIdElement.GetString();
            Console.WriteLine($"Project ID: {projectId}");
        }
        else
            Console.WriteLine("Project ID property not found.");
    }

    // GoogleCredential.GetApplicationDefault() automatically looks for:
    // 1. GOOGLE_APPLICATION_CREDENTIALS environment variable
    // 2. Default credentials from Google Cloud environments (e.g., App Engine, Cloud Run)
    // 3. gcloud CLI credentials (for local development)
    GoogleCredential credential = GoogleCredential.GetApplicationDefault();

    FirestoreDbBuilder dbBuilder = new FirestoreDbBuilder
    {
        ProjectId = projectId,
        Credential = credential
    };
    FirestoreDb db = dbBuilder.Build();
    builder.Services.AddSingleton(db); // Register FirestoreDb as a singleton
    Console.WriteLine("Firestore initialized successfully using Application Default Credentials.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error initializing Firestore: {ex.Message}");
    Console.WriteLine("Ensure GOOGLE_APPLICATION_CREDENTIALS environment variable is set and points to your service account key file.");
}

builder.Services.AddScoped<UserService>();

// Register your FirestoreAssetService here
builder.Services.AddScoped<IAssetService, FirestoreAssetService>();

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