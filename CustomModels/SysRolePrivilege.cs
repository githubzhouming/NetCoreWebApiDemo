using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class SysRolePrivilege : MyEntityBase
    {
        public override object getId()
        {
            return this.SysRolePrivilegeId;
        }

        public override string getIdName()
        {
            return "SysRolePrivilegeId";
        }

        public override string getTableName()
        {
            return "SysRolePrivilege";
        }
    }
}
