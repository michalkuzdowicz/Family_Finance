using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Family_Finance.Models;

public class FinancialTarget
{
    [Key]
    public int Id { get; set; }

    [Required]
    [DisplayName("Nazwa")]
    public string Name { get; set; }

    [MaxLength(255)]
    [DisplayName("Opis")]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [DisplayName("Docelowy stan")]
    public decimal TargetAmount { get; set; }

    [DisplayName("Aktualny Stan")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentAmount
    {
        get
        {
            return TargetTransactions?.Sum(t =>
                t.Type == "Income" ? t.Amount :
                t.Type == "Expense" ? -t.Amount : 0) ?? 0;
        }
        set { }
    }

    [Required]
    [DisplayName("Data utworzenia")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public int? FamilyGroupID { get; set; }
    public FamilyGroup FamilyGroup { get; set; }
    
    public ICollection<TargetTransaction> TargetTransactions { get; set; } 
   
}