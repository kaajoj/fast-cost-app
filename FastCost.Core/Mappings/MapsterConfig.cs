using FastCost.Core.DAL.Entities;
using FastCost.Core.Models;
using Mapster;

namespace FastCost.Core.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Cost, CostModel>.NewConfig();
            TypeAdapterConfig<Category, CategoryModel>.NewConfig();

            // CostModel -> Cost: ignore the navigation property so EF doesn't try to insert a duplicate Category
            TypeAdapterConfig<CostModel, Cost>.NewConfig()
                .Ignore(dest => dest.Category!);
        }
    }
}