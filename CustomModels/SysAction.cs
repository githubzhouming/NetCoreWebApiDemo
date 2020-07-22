using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class SysAction : MyEntityBase
    {
        public override object getId()
        {
            return this.SysActionId;
        }

        public override string getIdName()
        {
            return "SysActionId";
        }

        public override string getTableName()
        {
            return "SysAction";
        }
    }
}
