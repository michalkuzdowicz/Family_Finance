using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Family_Finance.Models;

public class FinancialTarget
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public decimal TargetAmount { get; set; }

    [Required]
    public decimal CurrentAmount { get; set; }
    
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public int? FamilyGroupID { get; set; }
    public FamilyGroup FamilyGroup { get; set; }
    
   public ICollection<TargetTransaction> TargetTransactions { get; set; }
    
   
}