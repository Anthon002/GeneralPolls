using GeneralPolls.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GeneralPolls.Core.Models;
using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Application.Services.Classes;
using GeneralPolls.Application.IRepositories;
using GeneralPolls.Infrastructure.Repositories;
using Hangfire;
using GeneralPolls.Core.OptionsSetup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//add database through the applicationdbcontext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineConnection"), b => b.MigrationsAssembly("GeneralPolls.Infrastructure"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;

    }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<RoleSeederService>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IGeneralPolls, GeneralPollsService>();
builder.Services.AddScoped<IGeneralPollsRepository, GeneralPollsRepository>();
builder.Services.AddHangfire((sp,config)=>{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("OnlineConnection");
    config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();
builder.Services.ConfigureOptions<ConfigurationOptionsSetup>();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}");

app.Run();
