using Family_Finance.Data;
using Family_Finance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Family_Finance.Controllers
{
    public class FamilyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {

        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;


        ////////////////\\\\\\\\\\\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
        /////// Manage My Family \\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var familyGroup = _context.FamilyGroups
                .Include(f => f.Members)
                .FirstOrDefault(f => f.HeadOfFamilyID == userId);
            if (familyGroup == null)
            {
                return NotFound();
            }

            // Upewnij się, że użytkownik jest głową rodziny
            var isHeadOfFamily = familyGroup.HeadOfFamilyID == userId;

            ViewData["IsHeadOfFamily"] = isHeadOfFamily;

            return View(familyGroup);
        }


        [HttpPost]
        public async Task<IActionResult> RemoveMember(string userId)
        {
            var member = await _userManager.FindByIdAsync(userId);

            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction("Index");
            }

            // Usunięcie użytkownika z rodziny
            member.FamilyGroupID = null;
            await _userManager.UpdateAsync(member);

            TempData["SuccessMessage"] = "Family member removed successfully!";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DivorceFamily()
        {
            var userId = User.Identity.Name;

            // Pobierz rodzinę, której użytkownik jest głową rodziny
            var familyGroup = await _context.FamilyGroups
                .Include(fg => fg.FinancialTargets)
                .Include(fg => fg.Members)
                .FirstOrDefaultAsync(fg => fg.HeadOfFamilyID == userId);

            if (familyGroup == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Usuń powiązane rekordy w tabeli FinancialTarget
            foreach (var target in familyGroup.FinancialTargets)
            {
                _context.FinancialTarget.Remove(target);
            }

            // Usuń członków rodziny (oprócz głowy rodziny)
            foreach (var member in familyGroup.Members)
            {
                if (member.Id != familyGroup.HeadOfFamilyID)
                {
                    var user = await _context.Users.FindAsync(member.Id);
                    if (user != null)
                    {
                        user.FamilyGroupID = null;
                        _context.Update(user);
                    }
                }
            }

            // Usuń rodzinę
            _context.FamilyGroups.Remove(familyGroup);

            // Zapisz zmiany w bazie danych
            await _context.SaveChangesAsync();

            // Przekieruj użytkownika na stronę główną lub do innej odpowiedniej akcji
            return RedirectToAction("Index", "Home");
        }






        ////////////////\\\\\\\\\\\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
        ////// Creating new family \\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
        [Authorize]
        public IActionResult CreateIndex()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFamily(string familyName)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.FamilyGroupID != null)
            {
                ModelState.AddModelError("", "You are already in the family.");
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

            TempData["SuccessMessage"] = "Family created successfully!";
            return RedirectToAction("Index", "Family");
        }





        ////////////////\\\\\\\\\\\\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\\
        /// INVITING NEW FAMILY MEMBER \\\
        ////////////////\\\\\\\\\\\\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\\
        public async Task<IActionResult> SendInvitation(string inviteeEmail)
        {
            var inviter = await _userManager.GetUserAsync(User);
            var invitee = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == inviteeEmail);

            if (invitee == null)
            {
                TempData["ErrorMessage"] = "No user found with this email.";
                return RedirectToAction("Index", "Family");
            }

            // Sprawdzanie, czy zapraszany użytkownik nie należy już do rodziny
            if (invitee.FamilyGroupID != null)
            {
                TempData["ErrorMessage"] = "This user is already part of a family.";
                return RedirectToAction("Index", "Family");
            }

            // Pobranie aktualnej rodziny głowy rodziny
            var familyGroup = await _context.FamilyGroups
                .FirstOrDefaultAsync(f => f.HeadOfFamilyID == inviter.Id); // Przykład, w zależności od struktury bazy danych

            if (familyGroup == null)
            {
                TempData["ErrorMessage"] = "You are not part of any family.";
                return RedirectToAction("Index", "Home");
            }

            // Tworzenie zaproszenia
            var invitation = new FamilyInvitation
            {
                InvitationDate = DateTime.Now,
                InviteeEmail = inviteeEmail,
                InviterID = inviter.Id,
                FamilyGroupID = familyGroup.ID, // Przypisanie rodziny do zaproszenia
                IsAccepted = false
            };

            _context.FamilyInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Invitation sent successfully!";
            return RedirectToAction("Index", "Family");

        }


        //
        // Wyświetlanie zaproszeń użytkownika
        //
        [Authorize]
        public async Task<IActionResult> MyInvitations()
        {
            var userId = _userManager.GetUserId(User);

            var invitations = await _context.FamilyInvitations
                .Include(i => i.FamilyGroup)
                .Where(i => i.InviteeEmail == User.Identity.Name) 
                .ToListAsync();

            if (invitations == null || invitations.Count == 0)
            {
                TempData["ErrorMessage"] = "No invitations found.";
            }

            return View(invitations);
        }



        //
        // Akceptowanie zaproszenia
        //
        public async Task<IActionResult> AcceptInvitation(int id)
        {
            var invitation = await _context.FamilyInvitations
                .Include(i => i.Inviter)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (invitation == null || invitation.IsAccepted)
            {
                TempData["ErrorMessage"] = "Invitation not found or already accepted.";
                return RedirectToAction("Index", "Home");
            }

            if (invitation.Inviter == null || invitation.Inviter.FamilyGroupID == null)
            {
                TempData["ErrorMessage"] = "Invalid invitation: Inviter or family group not found.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            // Przypisanie użytkownika do grupy rodzinnej zapraszającego
            user.FamilyGroupID = invitation.Inviter.FamilyGroupID;
            invitation.IsAccepted = true;

            // Aktualizacja danych w bazie
            _context.Update(user);
            _context.Update(invitation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Successfully joined the family!";
            return RedirectToAction("Index", "Finance");
        }

        //
        // Odrzucenie zaproszenia
        //
        public async Task<IActionResult> RejectInvitation(int id)
        {
            var invitation = await _context.FamilyInvitations
                .Include(i => i.Inviter)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (invitation == null || invitation.IsAccepted)
            {
                TempData["ErrorMessage"] = "Invitation not found or already accepted.";
                return RedirectToAction("Index", "Home");
            }

            // Usunięcie zaproszenia
            _context.FamilyInvitations.Remove(invitation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Invitation rejected successfully!";
            return RedirectToAction("Index", "Finance");
        }

    }
}
