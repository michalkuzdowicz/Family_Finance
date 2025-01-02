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
    
    public virtual ICollection<TargetTransaction> TargetTransactions { get; set; } = new List<TargetTransaction>();
    
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}