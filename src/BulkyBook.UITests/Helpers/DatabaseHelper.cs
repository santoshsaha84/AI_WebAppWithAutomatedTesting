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
            string port = databaseName switch
            {
                "Bulky" => "5434",
                "Bulky_1" => "5435",
                _ => throw new ArgumentException($"Unknown database name: {databaseName}")
            };
            string connectionString = $"Host=localhost;Port={port};Database={databaseName};Username=postgres;Password=postgres;Pooling=False";
            string masterConnectionString = $"Host=localhost;Port={port};Database=postgres;Username=postgres;Password=postgres;Pooling=False";

            Console.WriteLine($"[DatabaseHelper] Resetting database '{databaseName}' on port {port}...");

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<BulkyContext>();
                optionsBuilder.UseNpgsql(masterConnectionString);

                using (var context = new BulkyContext(optionsBuilder.Options))
                {
                    // Terminate active connections to allow dropping
                    Console.WriteLine($"[DatabaseHelper] Terminating active connections to '{databaseName}'...");
                    context.Database.ExecuteSqlRaw($@"
                        SELECT pg_terminate_backend(pg_stat_activity.pid)
                        FROM pg_stat_activity
                        WHERE pg_stat_activity.datname = '{databaseName}'
                          AND pid <> pg_backend_pid();");

                    Console.WriteLine($"[DatabaseHelper] Dropping and recreating '{databaseName}'...");
                    context.Database.ExecuteSqlRaw($"DROP DATABASE IF EXISTS \"{databaseName}\";");
                    context.Database.ExecuteSqlRaw($"CREATE DATABASE \"{databaseName}\";");
                }

                // Connect to the new database to seed it
                var seedOptionsBuilder = new DbContextOptionsBuilder<BulkyContext>();
                seedOptionsBuilder.UseNpgsql(connectionString);

                using (var context = new BulkyContext(seedOptionsBuilder.Options))
                {
                    Console.WriteLine($"[DatabaseHelper] Seeding schema and data...");
                    context.Database.EnsureCreated();

                    // Manually synchronize migration history
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\"MigrationId\" character varying(150) NOT NULL, \"ProductVersion\" character varying(32) NOT NULL, CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY (\"MigrationId\"));");
                    context.Database.ExecuteSqlRaw("INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20260111145233_InitialCreate', '7.0.16');");
                    context.Database.ExecuteSqlRaw("INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20260112100025_AddVendorTable', '7.0.16');");

                    SeedCategories(context);
                    SeedVendors(context);

                    if (databaseName == "Bulky")
                        SeedDatabase1(context);
                    else
                        SeedDatabase2(context);
                    
                    Console.WriteLine($"[DatabaseHelper] Database '{databaseName}' reset successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DatabaseHelper] ERROR resetting database '{databaseName}': {ex.Message}");
                throw;
            }
        }

        private static void SeedCategories(BulkyContext context)
        {
            context.Categories.AddRange(
                new Category { Name = "History", DisplayOrder = 1 },
                new Category { Name = "Geograph", DisplayOrder = 1 },
                new Category { Name = "Math", DisplayOrder = 1 }
            );
            context.SaveChanges();
        }

        private static void SeedVendors(BulkyContext context)
        {
            context.Vendors.AddRange(
                new Vendor { Name = "Rajkamal", City = "Hyderabad", Address = "Koti" },
                new Vendor { Name = "Seed Vendor", City = "Seed City", Address = "Seed Address" }
            );
            context.SaveChanges();
        }

        private static void SeedDatabase1(BulkyContext context)
        {
            // Specifically for Group1 (Category) tests if needed
            // But we already have seeded categories.
        }

        private static void SeedDatabase2(BulkyContext context)
        {
            // Seed a product for deletion test in ProductTests
            context.Products.Add(new Product
            {
                Title = "xya",
                Description = "Seeded for deletion",
                Price = 100,
                Price100 = 100,
                Price50 = 100,
                ListPrice = 100,
                Author = "Seeder",
                CategoryId = context.Categories.First(u => u.Name == "History").Id,
                Isbn = "SEED-DEL-1",
                ImageUrl = ""
            });
            context.SaveChanges();
        }
    }
}
