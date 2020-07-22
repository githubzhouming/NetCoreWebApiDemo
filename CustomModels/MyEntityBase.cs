using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public abstract class MyEntityBase
    {
        public abstract object getId();
        public abstract string getIdName();
        public abstract string getTableName();
    }
}
