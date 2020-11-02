using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Shared.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }



        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
