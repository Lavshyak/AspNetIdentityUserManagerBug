using AspNetIdentityUserManagerBug.Db;
using AspNetIdentityUserManagerBug.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// debug, launch profile: http, launchBrowser false.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MainDbContext>(options => options.UseInMemoryDatabase("mainDb"));

builder.Services.AddIdentity<Account, AccountRole>().AddSignInManager().AddEntityFrameworkStores<MainDbContext>();

var app = builder.Build();

// init database
using (var scope = app.Services.CreateScope())
{
    var mainDbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    mainDbContext.Database.EnsureCreated();
}

//create account and remember id
long accountId;
using (var scope = app.Services.CreateScope())
{
    var account = new Account()
    {
        Email = "test@test.com",
        UserName = "test"
    };

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();

    var result = await userManager.CreateAsync(account, "qwQW12!@");

    if (!result.Succeeded)
        throw new InvalidOperationException();

    accountId = account.Id;
}

// get account by id and remember it
Account accountToRemember;
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();
    var account = await userManager.FindByIdAsync(accountId.ToString());

    if (account == null)
        throw new InvalidOperationException();

    accountToRemember = account;
}

//trying to UpdateSecurityStamp with accountToRemember
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();
    
    try
    {
        var result = await userManager.UpdateSecurityStampAsync(accountToRemember);
        
        if (!result.Succeeded)
            throw new InvalidOperationException();
    }
    catch (InvalidOperationException ex)
    {
        if (ex.Message == "The instance of entity type 'Account' cannot be tracked " +
            "because another instance with the same key value for {'Id'} is already " +
            "being tracked. When attaching existing entities, ensure that only one " +
            "entity instance with a given key value is attached. Consider using " +
            "'DbContextOptionsBuilder.EnableSensitiveDataLogging' " +
            "to see the conflicting key values.")
        {
            Console.WriteLine("That's it");
        }

        throw;
    }
}
//app.Run();