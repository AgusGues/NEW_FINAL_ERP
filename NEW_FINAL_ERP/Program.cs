using System.Data;
using Microsoft.Data.SqlClient;
using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Repositories;
using NEW_FINAL_ERP.Repositories.Implementations;
using NEW_FINAL_ERP.Services;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");


// ==========================
// REGISTER DB CONNECTION
// ==========================
builder.Services.AddScoped<IDbConnection>(_ =>
    new SqlConnection(connString)
);


// ==========================
// UNIT OF WORK
// ==========================
builder.Services.AddScoped<UnitOfWork>();


// ==========================
// REPOSITORIES
// ==========================
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();


// ==========================
// SERVICES
// ==========================
builder.Services.AddScoped<NumberService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<BranchService>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Company}/{action=Index}/{id?}");

app.Run();
