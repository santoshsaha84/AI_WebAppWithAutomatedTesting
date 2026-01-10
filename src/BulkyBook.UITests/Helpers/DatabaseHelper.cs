using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TEST_DATA_SETUP.Data;
using TEST_DATA_SETUP.Models;

namespace BulkyBook.UITests.Helpers
{
    public static class DatabaseHelper
    {
        public static void ResetDatabaseToKnownState(string databaseName)
        {
            // Determine connection string based on database name (for Docker instances)
            string server = databaseName == "Bulky" ? "localhost,14330" : "localhost,14340";
            string connectionString = $"Server={server};Database=master;User Id=sa;Password=NinjaKing@100s00;TrustServerCertificate=True;";
            string databaseString = $"Server={server};Database={databaseName};User Id=sa;Password=NinjaKing@100s00;TrustServerCertificate=True;";

            // Backup file path INSIDE the container (based on volume mount)
            string backupFilePath = @"/var/opt/mssql/backup/Bulky_backup";

            // SQL commands
            string setSingleUser = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            string restoreDatabase = $"RESTORE DATABASE [{databaseName}] FROM DISK = '{backupFilePath}' WITH REPLACE, " +
                                     $"MOVE 'Bulky' TO '/var/opt/mssql/data/{databaseName}.mdf', " +
                                     $"MOVE 'Bulky_log' TO '/var/opt/mssql/data/{databaseName}_log.ldf'";
            string setMultiUser = $"ALTER DATABASE [{databaseName}] SET MULTI_USER";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Switch to SINGLE_USER
                    using (SqlCommand command = new SqlCommand(setSingleUser, connection))
                    {
                        try { command.ExecuteNonQuery(); } catch { /* Ignore if DB doesn't exist yet */ }
                    }

                    // Restore database
                    using (SqlCommand command = new SqlCommand(restoreDatabase, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Database [{databaseName}] restored successfully on {server}.");
                    }

                    // Switch back to MULTI_USER
                    using (SqlCommand command = new SqlCommand(setMultiUser, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Seed data using EF Core
                    var optionsBuilder = new DbContextOptionsBuilder<BulkyContext>();
                    optionsBuilder.UseSqlServer(databaseString);

                    using (var context = new BulkyContext(optionsBuilder.Options))
                    {
                        if (databaseName == "Bulky")
                            SeedDatabase1(context);
                        else
                            SeedDatabase2(context);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
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
                CategoryId = 2, // Assumed category ID 2 exists in backup
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
