using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Family_Finance.Models
{
    public class Transaction
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Type { get; set; }
        
        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public string UserID { get; set; }

        // Relacja do grupy/rodziny
        public int FamilyGroupID { get; set; }
        public FamilyGroup FamilyGroup { get; set; }
    }
}
