using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HonestAuto.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarketplaceContext>
    {
        public MarketplaceContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<MarketplaceContext>();
            var connectionString = configuration.GetConnectionString("MarketplaceContext");
            builder.UseSqlServer(connectionString);

            return new MarketplaceContext(builder.Options);
        }
    }
}