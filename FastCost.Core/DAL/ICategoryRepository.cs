using FastCost.Core.DAL.Entities;

namespace FastCost.Core.DAL
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategories();
        Task<Category?> GetCategory(int id);
        Task<int> SaveCategory(Category category);
        Task<int> DeleteCategory(Category category);
    }
}
