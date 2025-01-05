using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Family_Finance.Data;
using Family_Finance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Family_Finance.Controllers
{
    public class FinancialTargetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        // GET: FinancialTarget
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            
            return View(await _context.FinancialTarget
                .Where(ft => ft.FamilyGroupID == user.FamilyGroupID)
                .Include(ft => ft.TargetTransactions)
                .ToListAsync());
        }

        // GET: FinancialTarget/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (id == null)
            {
                return NotFound();
            }

            var financialTarget = await _context.FinancialTarget
                .Include(ft => ft.TargetTransactions)
                .FirstOrDefaultAsync(ft => ft.Id == id && ft.FamilyGroupID == user.FamilyGroupID);

            if (financialTarget == null)
            {
                return NotFound();
            }

            return View(financialTarget);
        }

        // GET: FinancialTarget/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FinancialTarget/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string description, decimal targetAmount, decimal currentAmount)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user.FamilyGroupID == null)
                {
                    return RedirectToAction("CreateFamily", "Family");
                }
                
                var financialTarget = new FinancialTarget
                {
                    Name = name,
                    Description = description,
                    TargetAmount = targetAmount,
                    FamilyGroupID = user.FamilyGroupID
                };

                _context.FinancialTarget.Add(financialTarget);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Target added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "An error occurred while updating the financial target. Please try again.");
                return View();
            }
        }

        // GET: FinancialTarget/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financialTarget = await _context.FinancialTarget.FindAsync(id);
            if (financialTarget == null)
            {
                return NotFound();
            }
            return View(financialTarget);
        }

        // POST: FinancialTarget/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string description, decimal targetAmount, decimal currentAmount)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                
                var financialTarget = await _context.FinancialTarget
                    .FirstOrDefaultAsync(ft => ft.Id == id && ft.FamilyGroupID == user.FamilyGroupID);

                if (financialTarget == null || user.FamilyGroupID == null)
                {
                    return NotFound();
                }

                financialTarget.Name = name;
                financialTarget.Description = description;
                financialTarget.TargetAmount = targetAmount;

                _context.Update(financialTarget);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Target updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinancialTargetExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                // Log the error here if you have logging configured
                ModelState.AddModelError("", "An error occurred while updating the financial target. Please try again.");
                return View();
            }
        }

        // GET: FinancialTarget/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financialTarget = await _context.FinancialTarget
                .FirstOrDefaultAsync(m => m.Id == id);
            if (financialTarget == null)
            {
                return NotFound();
            }

            return View(financialTarget);
        }

        // POST: FinancialTarget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var financialTarget = await _context.FinancialTarget.FindAsync(id);
            if (financialTarget != null)
            {
                _context.FinancialTarget.Remove(financialTarget);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Target deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool FinancialTargetExists(int id)
        {
            return _context.FinancialTarget.Any(e => e.Id == id);
        }
    }
}
