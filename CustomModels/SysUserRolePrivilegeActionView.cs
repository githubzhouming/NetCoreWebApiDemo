using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.CustomModels
{
    public class SysUserRolePrivilegeActionView
    {
        public Guid? SysUserId { get; set; }
        public Guid? SysRoleId { get; set; }
        public Guid? SysPrivilegeId { get; set; }
        public Guid? SysActionId { get; set; }
        public int? SysActionCode { get; set; }
        public string SysActionName { get; set; }
    }
}
