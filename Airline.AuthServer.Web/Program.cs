using Airline.AuthServer.Application;
using Airline.AuthServer.Infrastructure;
using Airline.AuthServer.Infrastructure.Persistence;
using Airline.AuthServer.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<Airline.AuthServer.Infrastructure.Identity.IdentityApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var applicationManager = services.GetRequiredService<IOpenIddictApplicationManager>();

    await context.Database.MigrateAsync();

    await SeedData.Initialize(
        userManager,
        roleManager,
        applicationManager,
        app.Configuration["OpenIddict:IssuerUri"]!
    );
}

app.Run();
