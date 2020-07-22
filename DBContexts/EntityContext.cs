using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebAPI.Common;

namespace WebAPI.DBContexts
{
    public class EntityContext: ZMDBContext
    {
        public EntityContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public EntityContext() : base()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new Exception ("数据库连接字符串未定义");
            }
        }
    }
}
