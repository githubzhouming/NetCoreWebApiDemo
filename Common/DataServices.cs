using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Auth;
using WebAPI.CustomModels;
using WebAPI.Models;
using ZM.Extensions.EntityFrameworkCoreEx;

namespace WebAPI.Common
{
    /// <summary>
    /// 数据执行
    /// </summary>
    public static class DataServices
    {
        public static IEnumerable<IpWhiteList> getUserIPWhiteList(DbContext dbContext, Guid userid)
        {

            var useridParameter = new Microsoft.Data.SqlClient.SqlParameter("@userid", userid);
            var UserIPWhiteList = dbContext.Database.SqlQueryDataTable<IpWhiteList>($"SELECT a.[IPWhiteListId],a.[urlList],[nameList] ,[SysUserId] FROM [IPWhiteList] a with(nolock) where a.SysUserId is null or a.SysUserId=@userid", useridParameter);
            return UserIPWhiteList;
        }
        public static IEnumerable<IpNameRef> getIpNameRef(DbContext dbContext, long ipValue)
        {

            var ipParameter = new Microsoft.Data.SqlClient.SqlParameter("@ip", ipValue);
            var ipNameRefList = dbContext.Database.SqlQueryDataTable<IpNameRef>($"SELECT  [IpNameRefId],[Country],[Local] FROM [ZMDB].[dbo].[IpNameRef] a with(nolock) where a.StartIPValue<=@ip and a.EndIPValue>=@ip", ipParameter);
            return ipNameRefList;
        }


        public static async Task<int> createAccessInfo(DbContext dbContext,Guid userid, string Url, string Path, string Ip, string Method, bool IsBlocked,string RequestKey)
        {
            var item = new AccessInfo()
            {
                SysUserId=userid,
                Url=Url,
                Path=Path,
                Ip=Ip,
                Method=Method,
                IsBlocked=IsBlocked,
                RequestKey=RequestKey
            };
            var entityEntry = dbContext.Entry(item);
            entityEntry.State = EntityState.Added;
            return await dbContext.saveChangesAsyncClientWin();
        }
        public static async Task<int> createActionAccessInfo(DbContext dbContext,string RequestKey)
        {
            var item = new ActionAccessInfo()
            {
                RequestKey=RequestKey
            };
            var entityEntry = dbContext.Entry(item);
            entityEntry.State = EntityState.Added;
            return await dbContext.saveChangesAsyncClientWin();
        }

        public static IEnumerable<IgnorePatternList> getUserIgnorePatternList(DbContext dbContext, Guid userid)
        {

            var useridParameter = new Microsoft.Data.SqlClient.SqlParameter("@userid", userid);
            var UserIPWhiteList = dbContext.Database.SqlQueryDataTable<IgnorePatternList>($"SELECT a.[IgnorePatternListId],a.[PatternList],[SysUserId] FROM [IgnorePatternList] a with(nolock) where a.SysUserId is null or a.SysUserId=@userid", useridParameter);
            return UserIPWhiteList;
        }

        public static IEnumerable<SysUserRolePrivilegeActionView> getUserActionList(DbContext dbContext, Guid userid)
        {
            var useridParameter = new Microsoft.Data.SqlClient.SqlParameter("@userid", userid);
            var userActions = dbContext.Database.SqlQueryDataTable<SysUserRolePrivilegeActionView>($"select distinct a.SysUserId,a.SysActionName from SysUserRolePrivilegeActionView a where a.SysUserId=@userid", useridParameter);
            return userActions;
        }
        public static IEnumerable<SysUserTableRight> getUserTableRigth(DbContext dbContext, Guid userid)
        {
            var useridParameter = new Microsoft.Data.SqlClient.SqlParameter("@userid", userid);
            var userTableRights = dbContext.Database.SqlQueryDataTable<SysUserTableRight>($"exec [dbo].[usp_right_UserTableName] @userid ", useridParameter);
            return userTableRights;
        }

        public static System.Data.DataTable getDataByRight<T>(DbContext dbContext, SqlAdvancedSelect sqlAdvancedSelect, Guid userId, T Tobj, bool isRigth) where T : MyEntityBase
        {
            sqlAdvancedSelect.mainTable = Tobj.getTableName();
            if (string.IsNullOrEmpty(sqlAdvancedSelect.orderStr))
            {
                sqlAdvancedSelect.orderStr =$"[{Tobj.getTableName()}].[{Tobj.getIdName()}]" ;
            }
            // if (sqlAdvancedSelect.fields == null || sqlAdvancedSelect.fields.Count == 0)
            // {
            //     sqlAdvancedSelect.fields = new List<string>();
            //     sqlAdvancedSelect.fields.Add("*");
            // }
            if (string.IsNullOrEmpty(sqlAdvancedSelect.whereStr))
            {
                sqlAdvancedSelect.whereStr = "1=1";
            }
            var whereRight = isRigth ? RightHelper.getReadWhereSql(userId, sqlAdvancedSelect.mainTable) : "1=1";
            var sql = sqlAdvancedSelect.getSql(whereRight);
            var result = dbContext.Database.SqlQueryDataTableCheckKey(sql);
            return result;
        }
    }
}
