using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Shared.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
