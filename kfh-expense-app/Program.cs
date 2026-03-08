using kfh_expense_app.Components;
using kfh_expense_app.Domain.Entities;
using kfh_expense_app.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using kfh_expense_app.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("ExpenseDb"));

builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//var jwt = builder.Configuration["Jwt:Key"]?? "masked_key_12345678";
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer= builder.Configuration["Jwt:Issuer"] ?? "masked_issuer",
//        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "masked_audience",
//        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt))
//    };
//});
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthSessionService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();



builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db=services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    db.Database.EnsureCreated();
    await DbSeeder.SeedAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
