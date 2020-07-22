using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public partial class IgnorePatternList : MyEntityBase
    {
        public override object getId()
        {
            return this.IgnorePatternListId;
        }

        public override string getIdName()
        {
            return "IgnorePatternListId";
        }

        public override string getTableName()
        {
            return "IgnorePatternList";
        }
    }
}
