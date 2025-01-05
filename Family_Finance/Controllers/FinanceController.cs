using Family_Finance.Data;
using Family_Finance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Family_Finance.Data.Migrations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace Family_Finance.Controllers
{
    public class FinanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [Authorize]
        public async Task<IActionResult> Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.FamilyGroupID == null)
            {
                return RedirectToAction("CreateFamily", "Family");
            }

            var query = _context.Transactions
                .Where(t => t.FamilyGroupID == user.FamilyGroupID);

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(t => t.Date <= endDate.Value);
            }

            var transactions = query.ToList();
            
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(transactions);
        }


        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate, int? familyGroupId)
        {
            var currentUserFamilyGroupId = _userManager.GetUserAsync(User).Result.FamilyGroupID; 

            if (familyGroupId == null)
            {
                familyGroupId = currentUserFamilyGroupId;
            }

            // Sprawdź, czy familyGroupId jest dostępny
            if (!familyGroupId.HasValue)
            {
                TempData["ErrorMessage"] = "No family group ID provided for the current user.";
                return RedirectToAction("Index");
            }

            var transactions = _context.Transactions.AsQueryable();

            if (startDate.HasValue)
            {
                startDate = startDate.Value.Date;
                transactions = transactions.Where(t => t.Date >= startDate);
            }

            if (endDate.HasValue)
            {
                endDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);
                transactions = transactions.Where(t => t.Date <= endDate);
            }

            // Filtruj transakcje po FamilyGroupID
            transactions = transactions.Where(t => t.FamilyGroupID == familyGroupId);

            var filteredTransactions = transactions.ToList();
            if (!filteredTransactions.Any())
            {
                TempData["ErrorMessage"] = "No transactions found in the selected period for this family.";
                return RedirectToAction("Index");
            }

            var fileName = "Transactions_" +
                           (startDate?.ToString("yyyyMMdd") ?? "All") +
                           "_" +
                           (endDate?.ToString("yyyyMMdd") ?? "All") +
                           ".xlsx";

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Transactions");

                // Dodaj nagłówki kolumn
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Value";
                worksheet.Cells[1, 3].Value = "Type";
                worksheet.Cells[1, 4].Value = "Date";
                worksheet.Cells[1, 5].Value = "User";

                var row = 2;
                foreach (var transaction in filteredTransactions)
                {
                    worksheet.Cells[row, 1].Value = transaction.Name;
                    worksheet.Cells[row, 2].Value = transaction.Amount;
                    worksheet.Cells[row, 3].Value = transaction.Type;
                    worksheet.Cells[row, 4].Value = transaction.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 5].Value = transaction.User;

                    row++;
                }

                // Wygeneruj plik
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
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
                FinancialTargetID = financialTargetId == 0 ? null : financialTargetId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();


            if (transaction.FinancialTargetID.HasValue)
            {
                var financialTarget = await _context.FinancialTarget
                       .FindAsync(transaction.FinancialTargetID.Value);

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

                //// Zaktualizowanie aktualnego stanu
                //financialTarget.UpdateAmount(targetTransaction.Amount, targetTransaction.Type);


                _context.TargetTransactions.Add(targetTransaction);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Transaction added successfully!";
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
        public async Task<IActionResult> UpdateTransaction(int id, string name, decimal amount, string type, string description, int financialTargetId)
        {
            var user = await _userManager.GetUserAsync(User);
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.ID == id && t.FamilyGroupID == user.FamilyGroupID);

            if (transaction == null)
            {
                return NotFound();
            }

            if (amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be greater than zero");
                return View("EditTransaction", transaction);
            }

            transaction.Name = name;
            transaction.Amount = amount;
            transaction.Type = type;
            transaction.Date = DateTime.UtcNow;
            transaction.Description = description;
            transaction.FinancialTargetID = financialTargetId == 0 ? null : financialTargetId;

            var targetTransaction = await _context.TargetTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == id);

            if (targetTransaction != null)
            {
                targetTransaction.Amount = amount;
                targetTransaction.Type = type;
                targetTransaction.Note = description;
            }
            else if (financialTargetId != 0)
            {
                var newTargetTransaction = new TargetTransaction
                {
                    Amount = amount,
                    TransactionDate = transaction.Date,
                    Note = description,
                    UserId = user.Id,
                    FinancialTargetId = financialTargetId,
                    TransactionId = id,
                    Type = type
                };
                _context.TargetTransactions.Add(newTargetTransaction);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Transaction updated successfully!";
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
            var targetTransaction = await _context.TargetTransactions.FirstOrDefaultAsync(t => t.TransactionId == transaction.ID);

            if (targetTransaction != null)
            {
                _context.TargetTransactions.Remove(targetTransaction);
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Transaction deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
