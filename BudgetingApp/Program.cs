using BudgetingApp.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Services to the Container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Needed for some internal routing

// Configure Session (Consolidated to one place)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15); // Balanced timeout for MFA entry
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".BudgetingApp.Session";
});

// Configure Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// Firestore Initialization Logic
try
{
    string jsonCredentialsString = builder.Configuration["GOOGLE_APPLICATION_CREDENTIALS_JSON"];

    if (string.IsNullOrEmpty(jsonCredentialsString))
    {
        string credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
        {
            jsonCredentialsString = File.ReadAllText(credentialsPath);
        }
        else
        {
            throw new InvalidOperationException("Google Credentials not found in Environment or App Settings.");
        }
    }

    string projectId = "";
    using (JsonDocument document = JsonDocument.Parse(jsonCredentialsString))
    {
        if (document.RootElement.TryGetProperty("project_id", out JsonElement projectIdElement))
        {
            projectId = projectIdElement.GetString();
        }
    }

    GoogleCredential credential = GoogleCredential.FromJson(jsonCredentialsString);
    FirestoreDb db = new FirestoreDbBuilder
    {
        ProjectId = projectId,
        Credential = credential
    }.Build();

    builder.Services.AddSingleton(db);
}
catch (Exception ex)
{
    Console.WriteLine($"Firestore Init Error: {ex.Message}");
    throw;
}

// Dependency Injection
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<AssetService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();