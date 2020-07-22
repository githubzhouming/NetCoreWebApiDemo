using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class SysPrivilege : MyEntityBase
    {
        public override object getId()
        {
            return this.SysPrivilegeId;
        }

        public override string getIdName()
        {
            return "SysPrivilegeId";
        }

        public override string getTableName()
        {
            return "SysPrivilege";
        }
    }
}
