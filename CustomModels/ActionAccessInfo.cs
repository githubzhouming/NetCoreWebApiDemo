using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class ActionAccessInfo : MyEntityBase
    {
        public override object getId()
        {
            return this.ActionAccessInfoId;
        }

        public override string getIdName()
        {
            return "ActionAccessInfoId";
        }

        public override string getTableName()
        {
            return "ActionAccessInfo";
        }
    }
}
