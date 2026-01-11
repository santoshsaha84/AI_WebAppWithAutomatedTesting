using System;
using Microsoft.EntityFrameworkCore;
using TEST_DATA_SETUP.Data;
using TEST_DATA_SETUP.Models;

namespace BulkyBook.UITests.Helpers
{
    public static class DatabaseHelper
    {
        public static void ResetDatabaseToKnownState(string databaseName)
        {
            // Determine port based on database name to match docker-compose ports (avoiding 5432/5433 to prevent conflicts)
            string port = databaseName == "Bulky" ? "5434" : "5435";
            string connectionString = $"Host=localhost;Port={port};Database={databaseName};Username=postgres;Password=postgres";

            var optionsBuilder = new DbContextOptionsBuilder<BulkyContext>();
            optionsBuilder.UseNpgsql(connectionString);

            using (var context = new BulkyContext(optionsBuilder.Options))
            {
                // Ensure the database is clean (Drop and Recreate)
                // This replaces the RESTORE DATABASE logic.
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Manually synchronize migration history so the background app doesn't crash trying to re-migrate
                context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\"MigrationId\" character varying(150) NOT NULL, \"ProductVersion\" character varying(32) NOT NULL, CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY (\"MigrationId\"));");
                context.Database.ExecuteSqlRaw("INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20260111145233_InitialCreate', '7.0.16');");

                // Seed Base Data (Categories) because BulkyContext does not have them by default 
                SeedCategories(context);

               // Seed Test Specific Data
                if (databaseName == "Bulky")
                    SeedDatabase1(context);
                else
                    SeedDatabase2(context);
            }
        }

        private static void SeedCategories(BulkyContext context)
        {
            // IDs are explicitly set to match what the tests might expect if they rely on IDs.
            // If Identity Insert is an issue, Npgsql handles it, or we rely on EF Core.
            context.Categories.AddRange(
                new Category { Id = 1, Name = "History", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Geograph", DisplayOrder = 1 },
                new Category { Id = 3, Name = "Math", DisplayOrder = 1 }
            );
            context.SaveChanges();
        }

        private static void SeedDatabase1(BulkyContext context)
        {
            context.Products.Add(new Product
            {
                Title = "xya",
                Description = "abc",
                Price = 100,
                Price100 = 100,
                Price50 = 100,
                ListPrice = 100,
                Author = "mnc",
                CategoryId = 2, 
                Isbn = "1234",
                ImageUrl = ""
            });
            context.SaveChanges();
        }

        private static void SeedDatabase2(BulkyContext context)
        {
            context.Products.Add(new Product
            {
                Title = "qpl",
                Description = "upi",
                Price = 40,
                Price100 = 60,
                Price50 = 90,
                ListPrice = 30,
                Author = "mnc",
                CategoryId = 2,
                Isbn = "12341234",
                ImageUrl = ""
            });
            context.SaveChanges();
        }
    }
}
