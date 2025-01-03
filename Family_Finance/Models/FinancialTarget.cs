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

    [Required]
    [DisplayName("Aktualny Stan")]
    public decimal CurrentAmount { get; private set; }
    
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public int? FamilyGroupID { get; set; }
    public FamilyGroup FamilyGroup { get; set; }
    
    public ICollection<TargetTransaction> TargetTransactions { get; set; } 

    public void UpdateAmount(decimal amount, string transactionType)
    {
        if (transactionType == "Income")
        {
            CurrentAmount += amount;
        }
        else if (transactionType == "Expense")
        {
            CurrentAmount -= amount;
        }
        else
        {
            throw new ArgumentException("Invalid transaction type", nameof(transactionType));
        }
    }
}