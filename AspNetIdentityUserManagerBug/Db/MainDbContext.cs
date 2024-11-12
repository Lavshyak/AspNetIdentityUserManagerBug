using AspNetIdentityUserManagerBug.Db.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetIdentityUserManagerBug.Db;

public class MainDbContext(DbContextOptions options) : IdentityDbContext<Account, AccountRole, long>(options)
{
}