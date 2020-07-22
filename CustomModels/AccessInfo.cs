using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class AccessInfo : MyEntityBase
    {
        public override object getId()
        {
            return this.AccessInfoId;
        }

        public override string getIdName()
        {
            return "AccessInfoId";
        }

        public override string getTableName()
        {
            return "AccessInfo";
        }
    }
}
