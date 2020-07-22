using System;

namespace WebAPI.Models
{
    public partial class ActionAccessInfo
    {
        public Guid ActionAccessInfoId { get; set; }
        
        public string RequestKey{get;set;}
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
