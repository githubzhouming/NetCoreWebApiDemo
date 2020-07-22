using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class IpNameRef : MyEntityBase
    {
        public override object getId()
        {
            return this.IpNameRefId;
        }

        public override string getIdName()
        {
            return "IpNameRefId";
        }

        public override string getTableName()
        {
            return "IpNameRef";
        }
    }
}
