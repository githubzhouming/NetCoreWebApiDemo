using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class SysUser : MyEntityBase
    {
        public override object getId()
        {
            return this.SysUserId;
        }

        public override string getIdName()
        {
            return "SysUserId";
        }

        public override string getTableName()
        {
            return "SysUser";
        }
    }
}
