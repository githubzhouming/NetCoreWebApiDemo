using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class SysRole : MyEntityBase
    {
        public override object getId()
        {
            return this.SysRoleId;
        }

        public override string getIdName()
        {
            return "SysRoleId";
        }

        public override string getTableName()
        {
            return "SysRole";
        }
    }
}
