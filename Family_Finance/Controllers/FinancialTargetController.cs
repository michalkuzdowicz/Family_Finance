using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Family_Finance.Data;
using Family_Finance.Models;

namespace Family_Finance.Controllers
{
    public class FinancialTargetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinancialTargetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FinancialTarget
        public async Task<IActionResult> Index()
        {
            return View(await _context.FinancialTarget.ToListAsync());
        }

        // GET: FinancialTarget/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description,TargetAmount,CurrentAmount,CreatedDate")] FinancialTarget financialTarget)
        {
            if (ModelState.IsValid)
            {
                _context.Add(financialTarget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,TargetAmount,CurrentAmount,CreatedDate")] FinancialTarget financialTarget)
        {
            if (id != financialTarget.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(financialTarget);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinancialTargetExists(financialTarget.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
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
