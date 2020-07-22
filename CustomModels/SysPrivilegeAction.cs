using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class SysPrivilegeAction : MyEntityBase
    {
        public override object getId()
        {
            return this.SysPrivilegeActionId;
        }

        public override string getIdName()
        {
            return "SysPrivilegeActionId";
        }

        public override string getTableName()
        {
            return "SysPrivilegeAction";
        }
    }
}
