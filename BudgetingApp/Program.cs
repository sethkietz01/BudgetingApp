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

try
{
    // Try to get JSON directly from an environment variable first
    string jsonCredentialsString = builder.Configuration["GOOGLE_APPLICATION_CREDENTIALS_JSON"];

    if (string.IsNullOrEmpty(jsonCredentialsString))
    {
        // Fallback to file path if the JSON variable isn't set (e.g., for local dev)
        string credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
        {
            jsonCredentialsString = File.ReadAllText(credentialsPath);
            Console.WriteLine("Successfully read GOOGLE_APPLICATION_CREDENTIALS file content.");
        }
        else
        {
            throw new InvalidOperationException("GOOGLE_APPLICATION_CREDENTIALS_JSON or GOOGLE_APPLICATION_CREDENTIALS file not found/set.");
        }
    }
    else
    {
        Console.WriteLine("Successfully read GOOGLE_APPLICATION_CREDENTIALS_JSON from App Settings.");
    }

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
            Console.WriteLine("Project ID property not found in JSON credentials.");
    }

    // Create credentials from the JSON string
    GoogleCredential credential = GoogleCredential.FromJson(jsonCredentialsString);

    FirestoreDbBuilder dbBuilder = new FirestoreDbBuilder
    {
        ProjectId = projectId,
        Credential = credential
    };
    FirestoreDb db = dbBuilder.Build();
    builder.Services.AddSingleton(db); // Register FirestoreDb as a singleton
    Console.WriteLine("Firestore initialized successfully using provided JSON credentials.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error initializing Firestore: {ex.Message}");
    throw new InvalidOperationException("Failed to initialize Firestore. Check App Service settings and Google Cloud credentials.", ex);
}


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AssetService>();
builder.Services.AddScoped<IAssetService, AssetService>();

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