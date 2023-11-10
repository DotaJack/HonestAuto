using HonestAuto.Models;
using Microsoft.EntityFrameworkCore;

namespace HonestAuto.Data
{// This is done so that the MarketplaceContext class can be used to interact with the database. This can then be paired with the Migrations which will allow the project to be used on other devices. Using Add-Migration DBcreation and then update-database
    public class MarketplaceContext : DbContext
    {
        public MarketplaceContext(DbContextOptions<MarketplaceContext> options) : base(options)
        {
            // Constructor for the MarketplaceContext class, which accepts DbContextOptions.
        }

        public DbSet<MessageConversation> MessageConversations { get; set; }
        // DbSet for the MessageConversation model. Allows interaction with the MessageConversations table.

        public DbSet<User> Users { get; set; }
        // DbSet for the User model. Allows interaction with the Users table.

        public DbSet<Mechanic> Mechanics { get; set; }
        // DbSet for the Mechanic model. Allows interaction with the Mechanics table.

        public DbSet<Car> Cars { get; set; }
        // DbSet for the Car model. Allows interaction with the Cars table.

        public DbSet<CarEvaluation> CarEvaluations { get; set; }
        // DbSet for the CarEvaluation model. Allows interaction with the CarEvaluations table.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Override the OnModelCreating method to configure entity relationships and constraints.

            modelBuilder.Entity<Car>()
                .HasMany(c => c.CarEvaluations)
                .WithOne(e => e.Car)
                .HasForeignKey(e => e.CarID);
            // Configure a one-to-many relationship between Car and CarEvaluations,
            // specifying that each Car can have multiple CarEvaluations.

            modelBuilder.Entity<CarEvaluation>()
                .HasOne(ce => ce.Mechanic)
                .WithMany(m => m.CarEvaluations)
                .HasForeignKey(ce => ce.MechanicID);
            // Configure a one-to-many relationship between CarEvaluation and Mechanic,
            // specifying that each Mechanic can have multiple CarEvaluations.
            // Also, define the foreign key relationship using MechanicID.

            // Ensure that this is the correct foreign key name when defining the relationship.
        }
    }
}