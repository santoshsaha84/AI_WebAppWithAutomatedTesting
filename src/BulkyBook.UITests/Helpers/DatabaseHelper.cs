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
                "Bulky_2" => "5436",
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
                    // EnsureCreated only creates tables, skips migrations
                    context.Database.EnsureCreated();

                    // Manually synchronize migration history
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\"MigrationId\" character varying(150) NOT NULL, \"ProductVersion\" character varying(32) NOT NULL, CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY (\"MigrationId\"));");
                    context.Database.ExecuteSqlRaw("INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20260111145233_InitialCreate', '7.0.16');");

                    SeedCategories(context);

                    if (databaseName == "Bulky")
                        SeedDatabase1(context);
                    else if (databaseName == "Bulky_1")
                        SeedDatabase2(context);
                    else
                        SeedDatabase3(context);
                    
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
        private static void SeedDatabase3(BulkyContext context)
        {
            // Seed a product for CrudTests (DeleteProduct_Success)
            context.Products.Add(new Product
            {
                Title = "xya",
                Description = "Seeded for deletion",
                Price = 100,
                Price100 = 100,
                Price50 = 100,
                ListPrice = 100,
                Author = "Seeder",
                CategoryId = 1,
                Isbn = "SEED-CRUD-1",
                ImageUrl = ""
            });
            context.SaveChanges();
        }
    }
}
