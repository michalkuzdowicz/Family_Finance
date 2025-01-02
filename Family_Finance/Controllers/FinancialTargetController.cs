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

namespace Family_Finance.Controllers
{
    public class FinancialTargetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        
        // GET: FinancialTarget
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            
            return View(await _context.FinancialTarget
                .Where(ft => ft.FamilyGroupID == user.FamilyGroupID)
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
                .FirstOrDefaultAsync(m => m.Id == id && m.FamilyGroupID == user.FamilyGroupID);
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
                CurrentAmount = currentAmount,
                FamilyGroupID = user.FamilyGroupID
            };

            _context.FinancialTarget.Add(financialTarget);
            await _context.SaveChangesAsync();
            
            return View(financialTarget);
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
            var user = await _userManager.GetUserAsync(User);
            
            var financialTarget = await _context.FinancialTarget
                .FirstOrDefaultAsync(ft => ft.Id == id && ft.FamilyGroupID == user.FamilyGroupID);

            
            if (id != financialTarget.Id)
            {
                return NotFound();
            }

           if (user.FamilyGroupID == null)
            {
                return RedirectToAction("CreateFamily", "Family");
            }
            
            var data = new FinancialTarget
            {
                Name = name,
                Description = description,
                TargetAmount = targetAmount,
                CurrentAmount = currentAmount,
                FamilyGroupID = user.FamilyGroupID
            };

            _context.FinancialTarget.Add(data);
            await _context.SaveChangesAsync();

            return View(financialTarget);
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
            return RedirectToAction(nameof(Index));
        }

        private bool FinancialTargetExists(int id)
        {
            return _context.FinancialTarget.Any(e => e.Id == id);
        }
    }
}
