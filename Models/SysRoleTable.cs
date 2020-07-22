using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysRoleTable
    {
        public Guid SysRoleTableId { get; set; }
        public Guid SysRoleId { get; set; }
        public string tableName { get; set; }
        public int permissionType { get; set; }
        public string searchSQL { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
