using HonestAuto.Models;
using Microsoft.EntityFrameworkCore;

namespace HonestAuto.Data

{
    public class MarketplaceContext : DbContext
    {
        public MarketplaceContext(DbContextOptions<MarketplaceContext> options) : base(options)
        {
        }

        public DbSet<MessageConversation> MessageConversations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Mechanic> Mechanics { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarEvaluation> CarEvaluations { get; set; }
    }
}