using System.Security.Claims;
using Newtonsoft.Json;

namespace PropelAuth.Models
{
    public class User
    {
        public string userId { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }

        public string legacyUserId { get; set; }

        public string impersonatorUserId { get; set; }
        public Dictionary<string, OrgMemberInfo> orgIdToOrgMemberInfo { get; set; }
        public Dictionary<string, object> properties { get; set; }
        public string loginMethod { get; set; }



        public User(ClaimsPrincipal claimsPrincipal)
        {
            userId = claimsPrincipal.FindFirstValue("user_id");
            email = claimsPrincipal.FindFirstValue("email");
            firstName = claimsPrincipal.FindFirstValue("first_name");
            lastName = claimsPrincipal.FindFirstValue("last_name");
            username = claimsPrincipal.FindFirstValue("username");
            legacyUserId = claimsPrincipal.FindFirstValue("legacy_user_id");
            impersonatorUserId = claimsPrincipal.FindFirstValue("impersonator_user_id");

            var orgsClaim = claimsPrincipal.FindFirst("org_id_to_org_member_info");
            if (orgsClaim != null)
            {
                orgIdToOrgMemberInfo = JsonConvert.DeserializeObject<Dictionary<string, OrgMemberInfo>>(orgsClaim.Value);
            }
            var propertiesClaim = claimsPrincipal.FindFirst("properties");
            if (propertiesClaim != null)
            {
                properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(propertiesClaim.Value);
            }

            var loginMethodClaim = claimsPrincipal.FindFirst("login_method");
            if (loginMethodClaim != null)
            {
                var loginMethodDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(loginMethodClaim.Value);
                loginMethod = loginMethodDict["login_method"];
            }

        }

        public bool IsRoleInOrg(string orgId, string role)
        {
            if (orgIdToOrgMemberInfo != null && orgIdToOrgMemberInfo.TryGetValue(orgId, out var orgInfo))
            {
                return orgInfo.user_role == role;
            }
            return false;
        }

        public bool IsAtLeastRoleInOrg(string orgId, string role)
        {
            if (orgIdToOrgMemberInfo != null && orgIdToOrgMemberInfo.TryGetValue(orgId, out var orgInfo))
            {
                return orgInfo.user_role == role ||
                       (orgInfo.inherited_user_roles_plus_current_role != null &&
                        orgInfo.inherited_user_roles_plus_current_role.Contains(role));
            }
            return false;
        }

        public bool HasPermissionInOrg(string orgId, string permission)
        {
            if (orgIdToOrgMemberInfo != null && orgIdToOrgMemberInfo.TryGetValue(orgId, out var orgInfo))
            {
                return orgInfo.user_permissions != null && orgInfo.user_permissions.Contains(permission);
            }
            return false;
        }

        public bool HasAllPermissionsInOrg(string orgId, string[] permissions)
        {
            if (orgIdToOrgMemberInfo != null && orgIdToOrgMemberInfo.TryGetValue(orgId, out var orgInfo))
            {
                if (orgInfo.user_permissions != null)
                {
                    return permissions.All(permission => orgInfo.user_permissions.Contains(permission));
                }
            }
            return false;
        }

        public OrgMemberInfo[] GetOrgs()
        {
            if (orgIdToOrgMemberInfo == null)
            {
                return Array.Empty<OrgMemberInfo>();
            }

            return orgIdToOrgMemberInfo.Values.ToArray();
        }

        public bool IsImpersonated()
        {
            return !string.IsNullOrEmpty(impersonatorUserId);
        }

        public OrgMemberInfo? GetOrg(string orgId)
        {
            if (orgIdToOrgMemberInfo != null && orgIdToOrgMemberInfo.TryGetValue(orgId, out var orgInfo))
            {
                return orgInfo;
            }
            return null;
        }

        public OrgMemberInfo? GetOrgByName(string orgName)
        {
            if (orgIdToOrgMemberInfo == null)
            {
                return null;
            }

            return orgIdToOrgMemberInfo.Values.FirstOrDefault(org => org.org_name.Equals(orgName, StringComparison.OrdinalIgnoreCase));
        }


    }

    public static class ClaimsPrincipalExtensions
    {
        public static User ToUser(this ClaimsPrincipal claimsPrincipal)
        {
            return new User(claimsPrincipal);
        }
    }
}
