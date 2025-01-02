namespace Family_Finance.Models
{
    public class FamilyGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string HeadOfFamilyID { get; set; }

        public ICollection<ApplicationUser> Members { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
