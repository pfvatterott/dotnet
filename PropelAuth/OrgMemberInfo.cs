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
    }
}