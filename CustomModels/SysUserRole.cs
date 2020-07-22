using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class SysUserRole : MyEntityBase
    {
        public override object getId()
        {
            return this.SysUserRoleId;
        }

        public override string getIdName()
        {
            return "SysUserRoleId";
        }

        public override string getTableName()
        {
            return "SysUserRole";
        }
    }
}
