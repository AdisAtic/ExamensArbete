using Microsoft.EntityFrameworkCore;
using PriceTrackingAPI.Models;

namespace PriceTrackingAPI.Data
{
    
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ProductPrice> ProductPrices { get; set; }
    }
}
