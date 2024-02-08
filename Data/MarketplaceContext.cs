using HonestAuto.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HonestAuto.Data
{// This is done so that the MarketplaceContext class can be used to interact with the database. This can then be paired with the Migrations which will allow the project to be used on other devices. Using Add-Migration DBcreation and then update-database
    public class MarketplaceContext : IdentityDbContext<User>
    {
        public MarketplaceContext(DbContextOptions<MarketplaceContext> options) : base(options)
        {
            // Constructor remains the same
        }

        public DbSet<Car> Cars { get; set; }
        // DbSet for the Car model. Allows interaction with the Cars table.

        public DbSet<CarEvaluation> CarEvaluations { get; set; }
        // DbSet for the CarEvaluation model. Allows interaction with the CarEvaluations table.

        public DbSet<ChatMessage> ChatMessages { get; set; }
        // DbSet for the CarEvaluation model. Allows interaction with the CarEvaluations table.

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Model> Models { get; set; }
    }
}