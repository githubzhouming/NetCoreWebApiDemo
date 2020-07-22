using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class IpNameRef
    {
        public Guid IpNameRefId { get; set; }
        public string StartIP { get; set; }
        public string EndIP { get; set; }
        public string Country { get; set; }
        public string Local { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] VerCol { get; set; }
    }
}
