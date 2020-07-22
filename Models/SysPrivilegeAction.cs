using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysPrivilegeAction
    {
        public Guid SysPrivilegeActionId { get; set; }
        public Guid SysPrivilegeId { get; set; }
        public Guid SysActionId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
