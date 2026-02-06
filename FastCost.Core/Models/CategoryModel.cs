namespace FastCost.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal? SumValue { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is CategoryModel other)
            {
                return Name == other.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}