using Family_Finance.Data;
using Family_Finance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Family_Finance.Controllers
{
    public class FamilyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
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

        public IActionResult CreateFamily()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFamily(string familyName)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.FamilyGroupID != null)
            {
                ModelState.AddModelError("", "Jesteś już w rodzinie.");
                return View();
            }

            var familyGroup = new FamilyGroup
            {
                Name = familyName,
                HeadOfFamilyID = user.Id,
                Members = [user]
            };

            _context.FamilyGroups.Add(familyGroup);
            await _context.SaveChangesAsync();

            user.FamilyGroupID = familyGroup.ID;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Finance"); // Przekierowanie do Finansów
        }



        ///
        /// TO JEST CENTRUM ZAPROSZENIOWE
        ///
        public IActionResult SendInvite()
        {
            return View();
        }

        // Wysyłanie zaproszenia
        public async Task<IActionResult> SendInvitation(string inviteeEmail)
        {
            var inviter = await _userManager.GetUserAsync(User);
            var invitee = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == inviteeEmail);

            if (invitee == null)
            {
                TempData["ErrorMessage"] = "No user found with this email.";
                return RedirectToAction("Index", "Home");
            }

            // Sprawdzanie, czy zapraszany użytkownik nie należy już do rodziny
            if (invitee.FamilyGroupID != null)
            {
                TempData["ErrorMessage"] = "This user is already part of a family.";
                return RedirectToAction("Index", "Home");
            }

            var invitation = new FamilyInvitation
            {
                InviterId = inviter.Id,
                InviteeEmail = inviteeEmail,
                IsAccepted = false,
                InvitationDate = DateTime.UtcNow
            };

            _context.FamilyInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // Wyświetlanie zaproszeń użytkownika
        public async Task<IActionResult> MyInvitations()
        {
            var user = await _userManager.GetUserAsync(User);

            // Załaduj zaproszenia i dołącz dane o zapraszającym
            var invitations = await _context.FamilyInvitations
                .Include(i => i.Inviter) // Dołącz zapraszającego
                .ThenInclude(u => u.FamilyGroup) // Dołącz rodzinę zapraszającego
                .Where(i => i.InviteeEmail == user.Email && !i.IsAccepted)
                .ToListAsync();

            return View(invitations);
        }

        // Akceptowanie zaproszenia
        public async Task<IActionResult> AcceptInvitation(int id)
        {
            var invitation = await _context.FamilyInvitations.FindAsync(id);

            if (invitation == null || invitation.IsAccepted)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            user.FamilyGroupID = invitation.Inviter.FamilyGroupID;
            invitation.IsAccepted = true;

            _context.Update(user);
            _context.Update(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Family");
        }

    }
}
