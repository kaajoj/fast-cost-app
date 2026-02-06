using FastCost.DAL.Entities;
using FastCost.Models;
using Mapster;

namespace FastCost.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Cost, CostModel>.NewConfig()
                .Map(dest => dest.Category, src => src.Category.Adapt<CategoryModel>());

            TypeAdapterConfig<Category, CategoryModel>.NewConfig();
        }
    }
}