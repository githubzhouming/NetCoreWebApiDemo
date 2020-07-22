using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysPrivilege
    {
        public Guid SysPrivilegeId { get; set; }
        public string Name { get; set; }
        public int PrivilegeType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
