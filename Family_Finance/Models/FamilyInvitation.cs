namespace Family_Finance.Models
{
    public class FamilyInvitation
    {
        public int Id { get; set; }
        public string InviterId { get; set; } // ID użytkownika zapraszającego
        public string InviteeEmail { get; set; } // Email zapraszanego użytkownika
        public bool IsAccepted { get; set; }
        public DateTime InvitationDate { get; set; }

        public ApplicationUser Inviter { get; set; } // Nawigacja do zapraszającego
    }
}
