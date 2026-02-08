using FastCost.Core.DAL.Entities;
using FastCost.Core.Models;
using Mapster;

namespace FastCost.Core.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Cost, CostModel>.NewConfig()
                .Map(dest => dest.Category, src => src.Category.Adapt<CategoryModel>());

            TypeAdapterConfig<Category, CategoryModel>.NewConfig();

            // New mapping from CostModel to Cost
            TypeAdapterConfig<CostModel, Cost>.NewConfig()
                .Map(dest => dest.CategoryId, src => src.CategoryId)
                .Ignore(dest => dest.Category); // Ignore mapping the Category navigation property
        }
    }
}