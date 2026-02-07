using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VetLink.Data.Contexts
{
    public class VetLinkDbContextFactory : IDesignTimeDbContextFactory<VetLinkDbContext>
    {
        public VetLinkDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<VetLinkDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("VetLinkDB"));

            return new VetLinkDbContext(optionsBuilder.Options);
        }
    }
}
