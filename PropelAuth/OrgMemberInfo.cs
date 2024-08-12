using System.Collections.Generic;

namespace PropelAuth.Models
{
    public class OrgMemberInfo
    {
        public string org_id { get; set; }
        public string org_name { get; set; }
        public string url_safe_org_name { get; set; }
        public Dictionary<string, object> org_metadata { get; set; }
        public string user_role { get; set; }
        public List<string> inherited_user_roles_plus_current_role { get; set; }
        public string org_role_structure { get; set; }
        public List<string> additional_roles { get; set; }
        public List<string> user_permissions { get; set; }

        public bool IsRole(string role)
        {
            return user_role == role;
        }
        public bool IsAtLeastRole(string role)
        {
            return user_role == role || (inherited_user_roles_plus_current_role != null && inherited_user_roles_plus_current_role.Contains(role));
        }
        public bool HasPermission(string permission)
        {
            return user_permissions != null && user_permissions.Contains(permission);
        }
        public bool HasAllPermissions(string[] permissions)
        {
            if (user_permissions != null)
            {
                return permissions.All(permission => user_permissions.Contains(permission));
            }
            return false;
        }
    }
}