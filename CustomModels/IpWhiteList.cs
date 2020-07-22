using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class IpWhiteList : MyEntityBase
    {
        public override object getId()
        {
            return this.IpwhiteListId;
        }

        public override string getIdName()
        {
            return "IpwhiteListId";
        }

        public override string getTableName()
        {
            return "IpWhiteList";
        }
    }
}
