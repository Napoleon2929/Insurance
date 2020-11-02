using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Shared.Roles
{
    public class SystemRoleClaims
    {
        public const string PERMISSION_CLAIM_TYPE = "permission";

        public static readonly Claim AccessManagement = new Claim(PERMISSION_CLAIM_TYPE, "access.management");
        public static readonly Claim UserRecordMonitoring = new Claim(PERMISSION_CLAIM_TYPE, "user.monitoring");
        public static readonly Claim UserRecordManagement = new Claim(PERMISSION_CLAIM_TYPE, "user.management");
        public static readonly Claim ClientUserRecordMonitoring = new Claim(PERMISSION_CLAIM_TYPE, "client_user.monitoring");
        public static readonly Claim ClientUserManagement = new Claim(PERMISSION_CLAIM_TYPE, "client_user.management");
    }
}
