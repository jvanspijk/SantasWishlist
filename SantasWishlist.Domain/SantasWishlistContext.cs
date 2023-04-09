using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SantasWishlist.Domain
{
    public class SantasWishlistContext : IdentityDbContext<SantasWishlistUser, IdentityRole, string>
    {
        public SantasWishlistContext(DbContextOptions<SantasWishlistContext> options) : base (options)
        {
            
        }

        //public DbSet<Gift> Gifts { get; set; }
        //public DbSet<WishList> WishLists { get; set; }
        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string fallbackConnStr = "Server=.;Database=SantasWishlist;Trusted_Connection=True;Trust Server Certificate=true;";

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(fallbackConnStr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //CreateManyToManyRelationships(modelBuilder);
            FillSeedData(modelBuilder);
        }

        private void FillSeedData(ModelBuilder modelBuilder)
        {
            
        }
    }
}
