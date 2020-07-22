using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebAPI.Common;
using WebAPI.CustomModels;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.DBContexts;
using ZM.Extensions.EntityFrameworkCoreEx;

namespace WebAPI.Auth
{
    /// <summary>
    /// 权限操作
    /// </summary>
    public static class RightHelper
    {
        private static IServiceCollection _services = null;
        private static IServiceProvider _serviceProvider = null;
        //}
        /// <summary>
        /// IServiceCollection
        /// </summary>
        /// <param name="str"></param>
        public static void init(IServiceCollection services)
        {
            if (_services != null)
            {
                throw new NotSupportedException("init cannot be call repeatedly");
            }
            _services = services;
            _serviceProvider = _services.BuildServiceProvider();
        }
        /// <summary>
        /// 用来缓存表权限信息
        /// </summary>
        private static Hashtable tableRightHashtable = new Hashtable();

        /// <summary>
        /// 默认没有权限返回
        /// </summary>
        private static readonly string NoneWhere = "(1=2)";
        /// <summary>
        /// 默认有权限返回
        /// </summary>
        private static readonly string AllWhere = "(1=1)";

        private static readonly Guid AdminID = new Guid("00000000-0000-0000-0000-000000000001");
        private static bool isAdmin(Guid userID)
        {
            return AdminID == userID;
        }
        #region Table权限
        private static Dictionary<string, SysUserTableRight> getTableRight(Guid userID)
        {
            Dictionary<string, SysUserTableRight> tableRightDic;
            if (tableRightHashtable.ContainsKey(userID))
            {
                tableRightDic = tableRightHashtable[userID] as Dictionary<string, SysUserTableRight>;
            }
            else
            {
                tableRightDic = null;
            }

            if (tableRightDic != null)
            {
                return tableRightDic;
            }
            lock (tableRightHashtable.SyncRoot)
            {
                tableRightDic = getTableRightByDB(userID);
                tableRightHashtable.Add(userID, tableRightDic);//加入缓存
                return tableRightDic;
            }
        }

        private static Dictionary<string, SysUserTableRight> getTableRightByDB(Guid userID)
        {
            IEnumerable<SysUserTableRight> tabltRights;
            using (IServiceScope serviceScope = _serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<EntityContext>())
                {
                    tabltRights = DataServices.getUserTableRigth(dbContext, userID);
                }
            }
            Dictionary<string, SysUserTableRight> userTableRightDic = new Dictionary<string, SysUserTableRight>();
            foreach (var tabltRight in tabltRights)
            {
                userTableRightDic.Add(tabltRight.getKey(), tabltRight);
            }
            return userTableRightDic;
        }

        private static string getWhereSql(Guid userID, string tableName, PermissionTypeEnum permissionType)
        {
            if (isAdmin(userID)) { return AllWhere; }
            var tableRightDic = getTableRight(userID);
            if (tableRightDic == null)
            {
                return NoneWhere;
            }
            var rightDic = new SysUserTableRight(userID, tableName, permissionType);
            if (tableRightDic.TryGetValue(rightDic.getKey(), out rightDic))
            {
                return rightDic.getWhereSql();
            }
            return NoneWhere;
        }

        /// <summary>
        /// 表权限，存在清空并返回true，不存在返回false
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static bool clearUserTableRight(Guid userID)
        {
            if (tableRightHashtable.ContainsKey(userID))
            {
                lock (tableRightHashtable.SyncRoot)
                {
                    tableRightHashtable.Remove(userID);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 表权限，存在清空
        /// </summary>
        public static bool clearAllUserTableRight()
        {
            if (tableRightHashtable.Count > 0)
            {
                lock (tableRightHashtable.SyncRoot)
                {
                    tableRightHashtable.Clear();
                }
                return true;
            }
            return false;
        }

        public static string getReadWhereSql(Guid userID, string tableName)
        {
            return getWhereSql(userID, tableName, PermissionTypeEnum.Read);
        }
        public static string getWriteWhereSql(Guid userID, string tableName)
        {
            return getWhereSql(userID, tableName, PermissionTypeEnum.Write);
        }
        public static string getDeleteWhereSql(Guid userID, string tableName)
        {
            return getWhereSql(userID, tableName, PermissionTypeEnum.Delete);
        }
        public static bool checkCreateRight(Guid userID, string tableName)
        {
            if (isAdmin(userID)) { return true; }
            var customRightBaseDic = getTableRight(userID);
            if (customRightBaseDic == null)
            {
                return false;
            }
            var RightBaseDic = new SysUserTableRight(userID, tableName, PermissionTypeEnum.Create);
            if (customRightBaseDic.ContainsKey(RightBaseDic.getKey()))
            {
                return true;
            }
            return false;
        }

        private static string checkWriteDeleteTableRigth<T>(Guid userID, List<T> entityList, PermissionTypeEnum permissionType) where T : Models.MyEntityBase
        {
            if (permissionType != PermissionTypeEnum.Write && permissionType != PermissionTypeEnum.Delete)
            {
                return $"{permissionType}权限类型不是 Write 或 Delete";
            }
            Dictionary<string, List<object>> tableIdDic = new Dictionary<string, List<object>>();
            Dictionary<string, string> tableIdNameDic = new Dictionary<string, string>();
            foreach (var entity in entityList)
            {
                var tableName = entity.getTableName();
                var id = entity.getId();
                var idName = entity.getIdName();
                if (!tableIdDic.ContainsKey(tableName))
                {
                    tableIdDic.Add(tableName, new List<object>());
                }
                var idLsit = tableIdDic[tableName];
                idLsit.Add(id);

                if (!tableIdNameDic.ContainsKey(tableName))
                {
                    tableIdNameDic.Add(tableName, idName);
                }
            }
            var whereSQL = string.Empty;

            foreach (var tableids in tableIdDic)
            {
                var ids = tableids.Value;
                var tableName = tableids.Key;
                var idName = tableIdNameDic[tableName];
                switch (permissionType)
                {
                    case PermissionTypeEnum.Write:
                        whereSQL = getWriteWhereSql(userID, tableName);
                        break;
                    case PermissionTypeEnum.Delete:
                        whereSQL = getDeleteWhereSql(userID, tableName);
                        break;
                }
                var sqlStr = $"select count(1) from {tableName} with(nolock) where [{idName}] in ('{string.Join("','", ids)}') and {whereSQL}";
                int cnt = 0;
                using (IServiceScope serviceScope = _serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
                {
                    using (var dbContext = serviceScope.ServiceProvider.GetService<EntityContext>())
                    {
                        cnt = dbContext.Database.SqlQueryScalar<int>(sqlStr);
                    }
                }

                if (cnt != ids.Count)
                {
                    return ($"{{权限异常类型:{permissionType},权限异常描述:权限不够,用户OID:{userID},表名:{tableName}}}");
                }
            }


            return string.Empty;
        }

        public static string checkWriteTableRigth<T>(Guid userID, List<T> entityList) where T : Models.MyEntityBase
        {
            return checkWriteDeleteTableRigth<T>(userID, entityList, PermissionTypeEnum.Write);
        }
        public static string checkDeleteTableRigth<T>(Guid userID, List<T> entityList) where T : Models.MyEntityBase
        {
            return checkWriteDeleteTableRigth<T>(userID, entityList, PermissionTypeEnum.Delete);
        }

        #endregion
    }
}
