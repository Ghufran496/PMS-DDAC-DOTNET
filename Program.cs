using PMS_DDAC.Data;
using Microsoft.EntityFrameworkCore; // Add this using directive
using Microsoft.AspNetCore.Identity;
using PMS_DDAC.Models; // Add this using directive
using PMS_DDAC.Services;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);

// Register HttpClient
builder.Services.AddHttpClient();

// ✅ Register Azure Email Servic
builder.Services.AddTransient<IEmailSender, AzureEmailSender>();
builder.Services.AddTransient<AzureEmailSender>();

// Register EF Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));

builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // 🚨 Ensures email confirmation is required before login
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Register RoleManager
builder.Services.AddScoped<RoleManager<IdentityRole>>();

//Add Razor Pages Services
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Owner", "Tenant" };

    foreach (var role in roles)
    {
        bool exists = await roleManager.RoleExistsAsync(role);
        if (!exists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// ✅ Ensure roles exist at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRoles(services);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
app.UseStaticFiles();
app.MapRazorPages();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
