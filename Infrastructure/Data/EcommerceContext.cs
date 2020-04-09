using System.Linq;
using System.Reflection;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class EcommerceContext : DbContext
    {
        public EcommerceContext(DbContextOptions<EcommerceContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //Workaround for sqlite as it doesn't support decimal.
            if(Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach(var item in modelBuilder.Model.GetEntityTypes())
                {
                    var props = item.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));
                    foreach(var prop in props)
                    {
                        modelBuilder.Entity(item.Name).Property(prop.Name).HasConversion<double>();
                    }
                }
            }
        }
    }
}