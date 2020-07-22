using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysUser
    {
        public Guid SysUserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
