using Duplex.Core.Models.Category;

namespace Duplex.Core.Contracts
{
    public interface ICategoryService
    {
        Task AddCategoryAsync(CategoryModel model);
        Task<IEnumerable<CategoryModel>> GetAllAsync();
        Task<CategoryModel> GetCategoryAsync(int cId);
        Task EditCategoryAsync(CategoryModel model);
        Task DeleteCategoryAsync(int cId);
        Task<bool> Exists(int cId);
    }
}
