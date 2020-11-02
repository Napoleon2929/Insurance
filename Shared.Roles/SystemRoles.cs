using System;
using System.Collections.Generic;
using System.Security.Claims;
using static Shared.Roles.SystemRoleClaims;

namespace Shared.Roles
{
    public static class SystemRoles
    {
        public const string EMAIL_CLAIM_TYPE = "email";
        public const string PHONE_NUMBER_CLAIM_TYPE = "phoneNumber";
        public const string FIRST_NAME_CLAIM_TYPE = "firstName";
        public const string LAST_NAME_CLAIM_TYPE = "lastName";
        public const string USER_NAME_CLAIM_TYPE = "userName";

        public const string AdminRoleName = "Admin";
        public const string IndividualPersonRoleName = "IndividualPerson";
        public const string LegalPersonRoleName = "LegalPerson";
        public const string InsuranceAgentRoleName = "InsuranceAgent";

        public static bool IsIndividualOrLegalRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException(nameof(role));
            }

            return string.Equals(role, IndividualPersonRoleName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, LegalPersonRoleName, StringComparison.OrdinalIgnoreCase);
        }

        public static Dictionary<string, IEnumerable<Claim>> GetSystemRolesWithClaims()
        {
            return new Dictionary<string, IEnumerable<Claim>>
            {
                {
                    AdminRoleName,
                    new []
                    {
                        AccessManagement,
                        UserRecordMonitoring,
                        UserRecordManagement,
                        ClientUserRecordMonitoring,
                        ClientUserManagement
                    }
                },
                {
                    InsuranceAgentRoleName,
                    new[]
                    {
                        ClientUserRecordMonitoring,
                        ClientUserManagement
                    }
                },
                {
                    IndividualPersonRoleName,
                    new List<Claim>()
                },
                {
                    LegalPersonRoleName,
                    new List<Claim>()
                }
            };
        }

    }
}
