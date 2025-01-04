using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace Family_Finance.Models
{
    public class Transaction
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Ilość")]
        public decimal Amount { get; set; }

        [Required]
        [DisplayName("Data")]
        public DateTime Date { get; set; }

        [Required]
        [DisplayName("Operacja")]
        public string Type { get; set; }
        
        [MaxLength(255)]
        [DisplayName("Opis")]
        public string? Description { get; set; }

        [Required]
        [DisplayName("Osoba")]
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }

        // Relacja do grupy/rodziny
        public int FamilyGroupID { get; set; }

        [ForeignKey("FamilyGroupID")]
        public FamilyGroup FamilyGroup { get; set; }

        public int? FinancialTargetID { get; set; }

        [ForeignKey("FinancialTargetID")]
        public FinancialTarget? FinancialTarget { get; set; }
    }
}
