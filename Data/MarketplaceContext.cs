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
    }
}