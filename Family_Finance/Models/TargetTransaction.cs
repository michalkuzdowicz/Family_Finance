using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Family_Finance.Models;

public class TargetTransaction
{
    [Key] public int Id { get; set; }

    [Required] public decimal Amount { get; set; }

    [Required] public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [Required] public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    [Required] public FinancialTarget FinancialTarget { get; set; }

    public string? Note { get; set; }
}