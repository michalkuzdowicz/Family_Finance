namespace Family_Finance.Models
{
    public class FamilyInvitation
    {
        public int Id { get; set; }
        public string InviterId { get; set; } // ID użytkownika zapraszającego
        public string InviteeEmail { get; set; } // Email zapraszanego użytkownika
        public string FamilyGroupId { get; set; } // Id grupy rodziny

        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public DateTime InvitationDate { get; set; }

        public FamilyGroup FamilyGroup { get; set; } // Nawiazanie do FamilyGroup
        public ApplicationUser Inviter { get; set; } // Nawigacja do zapraszającego
    }
}
