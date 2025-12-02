using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApplication8.Data
{
    public class IdentityAppContextFactory : IDesignTimeDbContextFactory<IdentityAppContext>
    {
        public IdentityAppContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityAppContext>();
            optionsBuilder.UseMySql(
                "server=localhost;port=3306;database=fauna;user=root;password=mila3454;", // <- поменяй на свои
                new MySqlServerVersion(new Version(8, 0, 34))
            );

            return new IdentityAppContext(optionsBuilder.Options);
        }
    }
}
