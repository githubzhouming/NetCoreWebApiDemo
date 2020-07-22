using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysRoleTableData
    {
        public Guid SysRoleTableDataId { get; set; }
        public string tableName { get; set; }
        public Guid SysRoleId { get; set; }
        public int permissionType { get; set; }
        public string searchSQL { get; set; }
        public DateTime? createdOn { get; set; }
        public DateTime? modifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
