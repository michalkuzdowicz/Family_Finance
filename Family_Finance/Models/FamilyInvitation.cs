using Microsoft.AspNetCore.Identity;

namespace Family_Finance.Models
{
    public class FamilyInvitation
    {
        public int ID { get; set; }
        public string InviterID { get; set; } // Klucz obcy do tabeli AspNetUsers
        public string InviteeEmail { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime InvitationDate { get; set; }
        public int? FamilyGroupID { get; set; } // Klucz obcy do tabeli FamilyGroups

        // Nawigacja do powiązanych encji
        public ApplicationUser Inviter { get; set; } // Relacja z użytkownikiem, który wysyła zaproszenie
        public FamilyGroup FamilyGroup { get; set; } // Relacja z grupą rodzinną
    }
}