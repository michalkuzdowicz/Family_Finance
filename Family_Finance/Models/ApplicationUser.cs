using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Family_Finance.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? FamilyGroupID { get; set; }
        public FamilyGroup FamilyGroup { get; set; }
    }
}
