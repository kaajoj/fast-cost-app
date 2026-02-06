using FastCost.DAL.Entities;

namespace FastCost.DAL
{
    interface ICategoryRepository
    {
        Task<List<Category>> GetCategories();
        Task<Category?> GetCategory(int id);
        Task<int> SaveCategory(Category category);
        Task<int> DeleteCategory(Category category);
    }
}
