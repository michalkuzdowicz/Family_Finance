using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Family_Finance.Models;

public class TargetTransaction
{
    [Key] 
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required] 
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string Type {  get; set; }
    
    public string? Note { get; set; }

    [Required] 
    public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    [Required]
    public int FinancialTargetId { get; set; }
    public FinancialTarget FinancialTarget { get; set; }

    [Required]
    public int TransactionId { get; set; }
    public Transaction Transaction { get; set; }
}