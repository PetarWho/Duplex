using Duplex.Core.Common;
using Duplex.Core.Models.Category;
using Duplex.Core.Services;
using Duplex.Infrastructure.Data.Models;

namespace Duplex.Tests
{
    public class CategoryServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddCategoryShouldWorkProperly()
        {
            var list = new List<Category>();

            var mockRepo = new Mock<IRepository>();

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback((Category category) => list.Add(category));

            var categoryService = new CategoryService(mockRepo.Object);

            var categoryModel = new CategoryModel()
            {
                Name = "Test from nUnit"
            };

            await categoryService.AddCategoryAsync(categoryModel);

            Assert.That(list.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task DeleteCategoryShouldWorkProperly()
        {
            var list = new List<Category>();

            var mockRepo = new Mock<IRepository>();

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback((Category category) => list.Add(category));

            var categoryService = new CategoryService(mockRepo.Object);

            var categoryModel = new CategoryModel()
            {
                Name = "Test from nUnit"
            };

            await categoryService.AddCategoryAsync(categoryModel);

            Assert.That(list.Count, Is.EqualTo(1));
        }
    }
}
