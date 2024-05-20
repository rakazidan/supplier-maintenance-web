using Microsoft.EntityFrameworkCore;
using SupplierMaintenanceWeb.Models.Entities;

namespace SupplierMaintenanceWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Location> Locations { get; set; }
    }
}
