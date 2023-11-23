using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HonestAuto.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarketplaceContext>
    {
        public MarketplaceContext CreateDbContext(string[] args)
        {
            // Implementing the IDesignTimeDbContextFactory interface to create a DbContext during design time.

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Build the configuration from the appsettings.json file.

            var builder = new DbContextOptionsBuilder<MarketplaceContext>();
            var connectionString = configuration.GetConnectionString("MarketplaceContext");
            builder.UseSqlServer(connectionString);

            // Configure the DbContext options with the SQL Server connection string obtained from appsettings.json.

            return new MarketplaceContext(builder.Options);
            // Create a new instance of the MarketplaceContext using the configured options.
        }
    }
}//https://stackoverflow.com/questions/50820275/how-to-use-idesigntimedbcontextfactory-implementation-in-asp-net-core-2-1