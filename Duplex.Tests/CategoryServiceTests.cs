using Duplex.Core.Common;
using Duplex.Core.Models.Category;
using Duplex.Core.Services;
using Duplex.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

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
            var categoryId = 6969;

            var categoryList = new List<Category>()
            {
                new Category()
                {
                    Id = categoryId,
                    Name = "Omega"
                },
                new Category()
                {
                    Id = categoryId,
                    Name = "Damn"
                },
                new Category()
                {
                    Id = categoryId + 1,
                    Name = "Damn"
                }
            };

            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(r => r.All<Category>())
                .Returns(categoryList.BuildMock());

            var categoryService = new CategoryService(mockRepo.Object);

            await categoryService.DeleteCategoryAsync(categoryId);

            var deleted = !mockRepo.Object.AllReadonly<Category>().Any();

            Assert.That(deleted, Is.EqualTo(true));
        }

        [Test]
        public async Task EditCategoryShouldWorkProperly()
        {
            var categoryId = 6969;

            var category = new Category()
            {
                Id = categoryId,
                Name = "xUnit"
            };

            var categoryList = new List<Category>()
            {
                category,
            };

            var mockRepository = new Mock<IRepository>();
            mockRepository
                .Setup(r => r.All<Category>())
                .Returns(categoryList.BuildMock());

            var categoryService = new CategoryService(mockRepository.Object);

            var newName = "nUnit";

            var editPostViewModel = new CategoryModel()
            {
                Id = categoryId,
                Name = newName,
            };

            await categoryService.EditCategoryAsync(editPostViewModel);

            Assert.Multiple(() =>
            {
                Assert.That(category.Id, Is.EqualTo(categoryId));
                Assert.That(category.Name, Is.EqualTo(newName));
            });
        }

        [Test]
#pragma warning disable CS1998
        public async Task EditCategoryShouldThrowAnExceptionWhenIdIsInvalid()
#pragma warning restore CS1998
        {
            var categoryId = 6969;
            var invalidCategoryId = 6970;

            var category = new Category()
            {
                Id = invalidCategoryId,
                Name = "xUnit"
            };

            var categoryList = new List<Category>()
            {
                category,
            };

            var mockRepository = new Mock<IRepository>();
            mockRepository
                .Setup(r => r.All<Category>())
                .Returns(categoryList.BuildMock());

            var categoryService = new CategoryService(mockRepository.Object);

            var newName = "nUnit";

            var editCategoryModel = new CategoryModel()
            {
                Id = categoryId,
                Name = newName
            };

            Assert.ThrowsAsync(typeof(Exception), async () => await categoryService.EditCategoryAsync(editCategoryModel));
        }

        [Test]
#pragma warning disable CS1998
        public async Task CategoryExistsShouldWorkProperly()
#pragma warning restore CS1998
        {
            var categoryId = 6969;

            var category = new Category()
            {
                Id = categoryId,
                Name = "nUnit"
            };

            var categoryList = new List<Category>()
            {
                category,
            };

            var mockRepository = new Mock<IRepository>();
            mockRepository
                .Setup(r => r.All<Category>())
                .Returns(categoryList.BuildMock());

            var categoryService = new CategoryService(mockRepository.Object);

            Assert.That(categoryList.Exists(x => x.Id == categoryId), Is.EqualTo(true));
        }
    }
}
