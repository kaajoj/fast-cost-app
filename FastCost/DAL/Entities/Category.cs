using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastCost.DAL.Entities
{
    [Table("categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }
    }
}