using Family_Finance.Data;
using Family_Finance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        public async Task<IActionResult> AddTransaction(string name, decimal amount, string type)
        {
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
                Date = DateTime.UtcNow,
                FamilyGroupID = user.FamilyGroupID.Value,
                UserID = user.Id
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
