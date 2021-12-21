using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data
{
    //session with the db, we needed to install entityFramework for this
    //we generate the database based on code (we have the entity Product)
    //we need the migration:
    //dotnet tool list -g -- entity framework tool
    //dotnet tool install --global dotnet-ef -version 5.0.4...
    //dotnet tool helps us with database, dbContex and migrations
    //dotnet ef migrations add InitialCreate -o Data/Migrations
    //it gennerated the files from Migration folder (Not this class)
    //to add manually some testing data:
    //dotnet ef database update (it creates or update database - skinet.db)
    //de sus din comm prompt sqlite open database - opens the sqlite explorer
    //right click on the table, new query and inserting the values
    //to drop the first database and creat a new migration
    //dotnet ef database drop -p Infrastructure -s API
    //dotnet ef migrations remove -p Infrastructure -s API
    //dotnet ef migrations add InitialCreate -p Infrastructure -s API -o Data/Migrations
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<Product> Products {get;set;}
        public DbSet<ProductType> ProductTypes {get;set;}
        public DbSet<ProductBrand> ProductBrands {get;set;}
        public DbSet<Order> Orders {get;set;}
        public DbSet<OrderItem> OrderItems {get;set;}
        public DbSet<DeliveryMethod> DeliveryMethods {get;set;}



        //see the ProductConfiguration file - it configures how we want to be a table, it method is looking for all IEntityTypeConfiguration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            if(Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach( var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));

                    var dateTimeOffsetProperties = entityType.ClrType.GetProperties().Where(
                         p => p.PropertyType == typeof(DateTimeOffset));

                    foreach(var property in properties)
                    {
                        modelBuilder.Entity(entityType.Name).Property(property.Name)
                        .HasConversion<double>();
                    }

                    foreach(var property in dateTimeOffsetProperties)
                    {
                        modelBuilder.Entity(entityType.Name).Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }

                }
            }
        }
    }
}