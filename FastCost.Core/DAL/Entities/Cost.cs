using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastCost.DAL.Entities
{
    [Table("costs")]
    public class Cost
    {
        [Key]
        public int Id { get; set; }

        public decimal Value { get; set; }
        
        public string? Description { get; set; }
        
        public DateTime Date { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
