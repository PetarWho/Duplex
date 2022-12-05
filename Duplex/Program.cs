using Duplex.Core.Common.Constants;
using Duplex.Data;
using Duplex.Extensions;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    //options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.AllowedUserNameCharacters = ProgramConstants.UserNameAllowedCharacters;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
    

builder.Services.ConfigureApplicationCookie(options =>
{
    //options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Errors/Error/_403";
});

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationServices();
builder.Services.AddSession();

builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = GoogleDriveConst.ClientId;
                    options.ClientSecret = GoogleDriveConst.ClientSecret;
                    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Errors/Error/_{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
          name: "areas",
          pattern: "{area:exists}/{controller=Error}/{action=NotFound}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
