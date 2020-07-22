using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class IpWhiteList
    {
        public Guid IpwhiteListId { get; set; }
        public string UrlList { get; set; }
        public string NameList { get; set; }
        public Guid? SysUserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
