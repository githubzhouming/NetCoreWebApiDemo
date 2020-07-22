using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysRole
    {
        public Guid SysRoleId { get; set; }
        public string Name { get; set; }
        public bool isAdmin{ get; set; }
        public Guid? ownerId{ get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
