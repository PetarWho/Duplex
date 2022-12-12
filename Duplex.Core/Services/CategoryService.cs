using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models.Category;
using Duplex.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository repo;
        public CategoryService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task AddCategoryAsync(CategoryModel model)
        {
            var category = new Category()
            {
                Name = model.Name
            };

            await repo.AddAsync(category);
            await repo.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int cId)
        {
            await repo.DeleteAsync<Category>(cId);

            await repo.SaveChangesAsync();
        }

        public async Task EditCategoryAsync(CategoryModel model)
        {
            var category = await repo.GetByIdAsync<Category>(model.Id);

            category.Name = model.Name;
            await repo.SaveChangesAsync();
        }

        public async Task<bool> Exists(int cId)
        {
            return await repo.AllReadonly<Category>().AnyAsync(e => e.Id == cId);
        }

        public async Task<IEnumerable<CategoryModel>> GetAllAsync()
        {
            var categories = await repo.AllReadonly<Category>().ToListAsync();

            return categories.Select(r => new CategoryModel()
            {
                Id = r.Id,
                Name = r.Name
            });
        }

        public async Task<CategoryModel> GetCategoryAsync(int cId)
        {
            var category = await repo.GetByIdAsync<Category>(cId);

            return new CategoryModel()
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
