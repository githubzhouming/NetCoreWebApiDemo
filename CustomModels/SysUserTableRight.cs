using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.CustomModels
{
    public class SysUserTableRight
    {
        public SysUserTableRight()
        {

        }
        public SysUserTableRight(Guid _SysUserId,string _tableName, PermissionTypeEnum _permissionType)
        {
            SysUserId = _SysUserId;
            tableName = _tableName;
            permissionType = _permissionType;
        }
        public Guid SysUserId { get; set; }
        public string tableName { get; set; }
        public string whereSql { get; set; }
        public PermissionTypeEnum permissionType { get; set; }

        public string getKey()
        {
            return SysUserId + tableName + permissionType;
        }
        public string getWhereSql()
        {
            return whereSql;
        }
    }
}
