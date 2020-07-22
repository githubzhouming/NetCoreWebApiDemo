using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class SysAction
    {
        public Guid SysActionId { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
