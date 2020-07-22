using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysUserRole
    {
        public Guid SysUserRoleId { get; set; }
        public Guid SysUserId { get; set; }
        public Guid SysRoleId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
