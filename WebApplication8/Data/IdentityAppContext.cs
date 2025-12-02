using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication8.Data
{
    public class IdentityAppContext : IdentityDbContext<IdentityUser>
    {
        public IdentityAppContext(DbContextOptions<IdentityAppContext> options)
            : base(options) { }
    }
}
