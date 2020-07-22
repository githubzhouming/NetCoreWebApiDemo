using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class IgnorePatternList
    {
        public Guid IgnorePatternListId { get; set; }
        public string PatternList { get; set; }
        public Guid? SysUserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
