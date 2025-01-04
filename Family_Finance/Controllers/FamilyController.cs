using Family_Finance.Data;
using Family_Finance.Models;
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
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

            return RedirectToAction("Index");
        }





        ////////////////\\\\\\\\\\\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
        ////// Creating new family \\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
        ////////////////\\\\\\\\\\\\\\\\\
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

            return RedirectToAction("Index", "Finance"); // Przekierowanie na stronę zarządzania rodziną
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
                return RedirectToAction("Index", "Finance");
            }

            // Sprawdzanie, czy zapraszany użytkownik nie należy już do rodziny
            if (invitee.FamilyGroupID != null)
            {
                TempData["ErrorMessage"] = "This user is already part of a family.";
                return RedirectToAction("Index", "Finance");
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

            return RedirectToAction("Index", "Finance"); // Przekierowanie na stronę zarządzania rodziną
        }


        //
        // Wyświetlanie zaproszeń użytkownika
        //
        public async Task<IActionResult> MyInvitations()
        {
            var user = await _userManager.GetUserAsync(User);

            var invitations = await _context.FamilyInvitations
                .Include(i => i.Inviter)
                .ThenInclude(u => u.FamilyGroup)
                .Where(i => i.InviteeEmail == user.Email && !i.IsAccepted)
                .ToListAsync();

            return View(invitations);
        }


        //
        // Akceptowanie zaproszenia
        //
        public async Task<IActionResult> AcceptInvitation(int id)
        {
            var invitation = await _context.FamilyInvitations
                .Include(i => i.Inviter)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invitation == null || invitation.IsAccepted)
            {
                return NotFound();
            }

            if (invitation.Inviter == null || invitation.Inviter.FamilyGroupID == null)
            {
                TempData["ErrorMessage"] = "Invalid invitation: Inviter or family group not found.";
                return RedirectToAction("MyInvitations");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("MyInvitations");
            }

            user.FamilyGroupID = invitation.Inviter.FamilyGroupID;
            invitation.IsAccepted = true;

            _context.Update(user);
            _context.Update(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Finance"); // Przekierowanie na stronę zarządzania rodziną
        }

        //
        // Odrzucenie zaproszenia
        //
        public async Task<IActionResult> RejectInvitation(int id)
        {
            var invitation = await _context.FamilyInvitations
                .Include(i => i.Inviter)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invitation == null || invitation.IsAccepted)
            {
                return NotFound();
            }

            // Opcjonalnie: Jeżeli chcesz usunąć zaproszenie z bazy po jego odrzuceniu
            _context.FamilyInvitations.Remove(invitation);

            // Jeżeli chcesz zachować zaproszenie, ale oznaczyć je jako odrzucone
            // invitation.IsRejected = true;
            // _context.Update(invitation);

            await _context.SaveChangesAsync();

            return RedirectToAction("MyInvitations"); // Przekierowanie z powrotem do strony zaproszeń
        }


    }
}
