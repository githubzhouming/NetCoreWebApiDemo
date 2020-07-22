using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class SysRoleTable : MyEntityBase
    {
        public override object getId()
        {
            return this.SysRoleTableId;
        }

        public override string getIdName()
        {
            return "SysRoleTableId";
        }

        public override string getTableName()
        {
            return "SysRoleTable";
        }
    }
}
