namespace Family_Finance.Models
{
    public class FamilyGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string HeadOfFamilyID { get; set; }

        public ICollection<ApplicationUser> Members { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<FinancialTarget> FinancialTargets { get; set; }
    }

    public class FamilyManageViewModel
    {
        public string FamilyGroupName { get; set; }
        public List<ApplicationUser> Members { get; set; }
        public bool IsHeadOfFamily { get; set; }
    }
}
