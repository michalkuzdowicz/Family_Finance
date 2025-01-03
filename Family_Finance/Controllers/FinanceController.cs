using Family_Finance.Data;
using Family_Finance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Family_Finance.Controllers
{
    public class FinanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.FamilyGroupID == null)
            {
                return RedirectToAction("CreateFamily", "Family");
            }

            var transactions = _context.Transactions
                .Where(t => t.FamilyGroupID == user.FamilyGroupID)
                .ToList();

            return View(transactions);
        }
        
        public async Task<IActionResult> CreateTransaction()
        {
            var user = await _userManager.GetUserAsync(User);

            var financialTargets = await _context.FinancialTarget
                .Where(ft => ft.FamilyGroupID == user.FamilyGroupID)
                .Select(ft => new SelectListItem
                {
                    Value = ft.Id.ToString(),
                    Text = ft.Name
                })
                .ToListAsync();

            var transaction = new Transaction
            {
                Date = DateTime.UtcNow
            };

            ViewBag.FinancialTargets = financialTargets;
            
            return View(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction(string name, decimal amount, string type, string description, int financialTargetId)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount cannot be zero");
                return View("CreateTransaction", new Transaction 
                { 
                    Name = name,
                    Amount = amount,
                    Type = type,
                    Description = description,
                    Date = DateTime.UtcNow
                });
            }

            var user = await _userManager.GetUserAsync(User);
            
            if (user.FamilyGroupID == null)
            {
                return RedirectToAction("CreateFamily", "Family");
            }

            var transaction = new Transaction
            {
                Name = name,
                Amount = amount,
                Type = type,
                Description = description,
                Date = DateTime.UtcNow,
                FamilyGroupID = (int) user.FamilyGroupID,
                UserID = user.Id,
                FinancialTargetID = financialTargetId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var financialTarget = await _context.FinancialTarget
                   .FindAsync(transaction.FinancialTargetID.Value);

            if (transaction.FinancialTargetID.HasValue && transaction.FinancialTargetID == financialTarget.Id)
            {
                var targetTransaction = new TargetTransaction
                {
                    Amount = transaction.Amount,
                    TransactionDate = transaction.Date,
                    Note = transaction.Description,
                    UserId = user.Id,
                    FinancialTargetId = (int)transaction.FinancialTargetID,
                    TransactionId = transaction.ID,
                    Type = transaction.Type,
                };

                // Zaktualizowanie aktualnego stanu
                financialTarget.UpdateAmount(targetTransaction.Amount, targetTransaction.Type);


                _context.TargetTransactions.Add(targetTransaction);
                await _context.SaveChangesAsync();
            }

            

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditTransaction(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.ID == id && t.FamilyGroupID == user.FamilyGroupID);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTransaction(int id, string name, decimal amount, string type, string description)
        {
            var user = await _userManager.GetUserAsync(User);
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.ID == id && t.FamilyGroupID == user.FamilyGroupID);

            if (transaction == null)
            {
                return NotFound();
            }

            if (amount == 0)
            {
                ModelState.AddModelError("Amount", "Amount cannot be zero");
                return View("EditTransaction", transaction);
            }

            transaction.Name = name;
            transaction.Amount = amount;
            transaction.Type = type;
            transaction.Date = DateTime.UtcNow;
            transaction.Description = description;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.ID == id && t.FamilyGroupID == user.FamilyGroupID);

            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
