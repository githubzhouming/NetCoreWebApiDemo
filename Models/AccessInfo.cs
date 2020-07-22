using System;

namespace WebAPI.Models
{
    public partial class AccessInfo
    {
        public Guid AccessInfoId { get; set; }
        public Guid? SysUserId { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public string Ip { get; set; }
        public string Method { get; set; }
        public bool IsBlocked{get;set;}

        public string RequestKey{get;set;}
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
