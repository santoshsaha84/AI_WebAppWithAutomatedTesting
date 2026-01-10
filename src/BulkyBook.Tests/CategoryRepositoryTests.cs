using BulkyBook.DataAccess.Repository;
using BulkyBook.Models;
using BulkyBookWeb.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BulkyBook.Tests
{
    public class CategoryRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void Add_WhenCalled_AddsCategory()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new ApplicationDbContext(options);
            var repository = new CategoryRepository(context);
            var category = new Category { Name = "Test Category", DisplayOrder = 1 };

            // Act
            repository.Add(category);
            context.SaveChanges();

            // Assert
            var result = context.Categories.FirstOrDefault(u => u.Name == "Test Category");
            Assert.NotNull(result);
            Assert.Equal(1, result.DisplayOrder);
        }

        [Fact]
        public void GetAll_WhenCalled_ReturnsAllCategories()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new ApplicationDbContext(options);
            context.Categories.Add(new Category { Name = "Cat 1", DisplayOrder = 1 });
            context.Categories.Add(new Category { Name = "Cat 2", DisplayOrder = 2 });
            context.SaveChanges();

            var repository = new CategoryRepository(context);

            // Act
            var result = repository.GetAll().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Name == "Cat 1");
            Assert.Contains(result, u => u.Name == "Cat 2");
        }
    }
}
