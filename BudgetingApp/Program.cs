using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Initialize Firestore using FirestoreDbBuilder to pick up credentials
string projectId = "budgetingdatabase"; 
FirestoreDbBuilder dbBuilder = new FirestoreDbBuilder
{
    ProjectId = projectId
};
FirestoreDb db = dbBuilder.Build();
builder.Services.AddSingleton(db); // Register FirestoreDb as a singleton

// Register your UserService (if you have one)
builder.Services.AddScoped<UserService>();

// Add authentication services (if using cookies)
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

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();  // Enable authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();