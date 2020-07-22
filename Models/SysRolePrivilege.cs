using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysRolePrivilege
    {
        public Guid SysRolePrivilegeId { get; set; }
        public Guid SysRoleId { get; set; }
        public Guid SysPrivilegeId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
