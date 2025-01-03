namespace Family_Finance.Models
{
    public class FamilyInvitation
    {
        public int Id { get; set; }
        public string InviterId { get; set; }
        public string InviteeEmail { get; set; }
        public int? FamilyGroupID { get; set; }

        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public DateTime InvitationDate { get; set; }

        public FamilyGroup FamilyGroup { get; set; }
        public ApplicationUser Inviter { get; set; }
    }
}
