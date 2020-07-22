using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using ZM.Extensions.DataEx;

namespace ZM.Extensions.EntityFrameworkCoreEx
{
    /// <summary>
    /// DbContextExtensions
    /// </summary>
    public static class DbContextExtensions
    {
        

        #region 使用 Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade
        public static IEnumerable<T> SqlQueryDataTable<T>(this DatabaseFacade facade, string sql, params DbParameter[] parameters) where T : class, new()
        {
            var dt = SqlQueryExecuteReader(facade, sql, parameters);
            return dt.ToEnumerable<T>();
        }
        public static IEnumerable<T> SqlQueryDataTableCheckKey<T>(this DatabaseFacade facade, string sql, params DbParameter[] parameters) where T : class, new()
        {
            if (!sql.checkSqlSelect())
            {
                throw new Exception("现敏感关键字:" + sql);
            }
            return SqlQueryDataTable<T>(facade, sql, parameters);
        }
        public static DataTable SqlQueryDataTable(this DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            var dt = SqlQueryExecuteReader(facade, sql, parameters);
            return dt;
        }
        public static DataTable SqlQueryDataTableCheckKey(this DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            if (!sql.checkSqlSelect())
            {
                throw new Exception("现敏感关键字:" + sql);
            }
            return SqlQueryDataTable(facade, sql, parameters);
        }

        public static T SqlQueryScalar<T>(this DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            var obj = SqlQueryExecuteScalar(facade, sql, parameters);
            return (T)obj;
        }
        public static T SqlQueryScalarCheckKey<T>(this DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            if (!sql.checkSqlSelect())
            {
                throw new Exception("现敏感关键字:" + sql);
            }
            return SqlQueryScalar<T>(facade, sql, parameters);
        }
        public static int SqlQueryNonQuery(this DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            var result = SqlQueryExecuteNonQuery(facade, sql, parameters);
            return result;
        }
        #endregion

        #region 使用 Microsoft.Extensions.DependencyInjection.IServiceCollection
        public static IEnumerable<K> SqlQueryDataTable<T, K>(this IServiceCollection services, string sql, params DbParameter[] parameters) where T : DbContext where K : class, new()
        {
            using (IServiceScope serviceScope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<T>())
                {
                    return dbContext.Database.SqlQueryDataTable<K>(sql, parameters);
                }
            }
        }
        public static IEnumerable<K> SqlQueryDataTableCheckKey<T, K>(this IServiceCollection services, string sql, params DbParameter[] parameters) where T : DbContext where K : class, new()
        {
            using (IServiceScope serviceScope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<T>())
                {
                    return dbContext.Database.SqlQueryDataTableCheckKey<K>(sql, parameters);
                }
            }
        }
        public static DataTable SqlQueryDataTable<T>(this IServiceCollection services, string sql, params DbParameter[] parameters) where T : DbContext
        {
            using (IServiceScope serviceScope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<T>())
                {
                    return dbContext.Database.SqlQueryDataTable(sql, parameters);
                }
            }
        }
        public static DataTable SqlQueryDataTableCheckKey<T>(this IServiceCollection services, string sql, params DbParameter[] parameters) where T : DbContext
        {
            using (IServiceScope serviceScope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<T>())
                {
                    return dbContext.Database.SqlQueryDataTableCheckKey(sql, parameters);
                }
            }
        }

        public static K SqlQueryScalar<T, K>(this IServiceCollection services, string sql, params DbParameter[] parameters) where T : DbContext where K : class, new()
        {
            using (IServiceScope serviceScope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<T>())
                {
                    return dbContext.Database.SqlQueryScalar<K>(sql, parameters);
                }
            }
        }
        public static K SqlQueryScalarCheckKey<T, K>(this IServiceCollection services, string sql, params DbParameter[] parameters) where T : DbContext where K : class, new()
        {
            using (IServiceScope serviceScope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<T>())
                {
                    return dbContext.Database.SqlQueryScalar<K>(sql, parameters);
                }
            }
        }
        public static int SqlQueryNonQuery<T>(this IServiceCollection services, string sql, params DbParameter[] parameters) where T : DbContext
        {
            using (IServiceScope serviceScope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<T>())
                {
                    return dbContext.Database.SqlQueryNonQuery(sql, parameters);
                }
            }
        }
        #endregion
        #region 私有帮助方法
        private static void CombineParams(DbCommand command, params DbParameter[] parameters)
        {
            CombineParams(command, null, parameters);
        }
        private static void CombineParams(DbCommand command, int? timeout, params DbParameter[] parameters)
        {
            if (timeout == null)
            {
                timeout = 60;
            }
            command.CommandTimeout = timeout.Value;
            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    if (!parameter.ParameterName.Contains("@"))
                        parameter.ParameterName = $"@{parameter.ParameterName}";
                    command.Parameters.Add(parameter);
                }
            }
        }


        private static DataTable SqlQueryExecuteReader(this DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            DataTable dt = new DataTable();
            DbConnection conn = facade.GetDbConnection();
            lock (conn)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (DbCommand cmd = conn.CreateCommand())
                {
                    if (facade.IsSqlServer())
                    {
                        cmd.CommandText = sql;
                        CombineParams(cmd, parameters);
                    }
                    else
                    {
                        cmd.CommandText = sql;
                        CombineParams(cmd, parameters);
                    }
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                        reader.Close();
                    }
                }
                conn.Close();
            }
            return dt;
        }

        private static object SqlQueryExecuteScalar(this DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            object result;
            DbConnection conn = facade.GetDbConnection();
            lock (conn)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (DbCommand cmd = conn.CreateCommand())
                {
                    if (facade.IsSqlServer())
                    {
                        cmd.CommandText = sql;
                        CombineParams(cmd, parameters);
                    }
                    else
                    {
                        cmd.CommandText = sql;
                        CombineParams(cmd, parameters);
                    }
                    result = cmd.ExecuteScalar();
                }
                conn.Close();
            }
            return result;
        }

        private static int SqlQueryExecuteNonQuery(this DatabaseFacade facade, string sql, params DbParameter[] parameters)
        {
            int result;
            DbConnection conn = facade.GetDbConnection();
            lock (conn)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (DbCommand cmd = conn.CreateCommand())
                {
                    if (facade.IsSqlServer())
                    {
                        cmd.CommandText = sql;
                        CombineParams(cmd, parameters);
                    }
                    else
                    {
                        cmd.CommandText = sql;
                        CombineParams(cmd, parameters);
                    }
                    result = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return result;
        }


        private static readonly List<string> sqlSelectKeyWordList = new List<string>() { "exec ", "exec[", "insert ", "insert[", "delete ", "delete[", "update ", "update[", "declare ", "declare[", "truncate ", "truncate[" };

        private static bool checkSqlSelect(this string sqlStr)
        {
            if (!string.IsNullOrWhiteSpace(sqlStr))
            {
                foreach (var keyStr in sqlSelectKeyWordList)
                {
                    if (sqlStr.IndexOf(keyStr, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region EF 自带方法提交数据


        /// <summary>
        /// 保存数据，数据库优先
        /// 并发冲突后，重新读取数据库数据
        /// </summary>
        /// <param name="dbContext"></param>
        public static async Task<int> saveChangesAsyncDBWin(this DbContext dbContext, int retryCount = 1)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    return await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    retryCount--;
                    if (retryCount == -1)
                    {
                        throw ex;
                    }
                    saveFailed = true;
                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
            return -1;
        }
        /// <summary>
        /// 保存数据，数据库优先
        /// 并发冲突后，重新读取数据库数据
        /// </summary>
        /// <param name="dbContext"></param>
        public static int saveChangesDBWin(this DbContext dbContext, int retryCount = 1)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    return dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    retryCount--;
                    if (retryCount == -1)
                    {
                        throw ex;
                    }
                    saveFailed = true;
                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
            return -1;
        }
        /// <summary>
        /// 保存数据，客户端程序优先
        /// 并发冲突后，将数据库值设置成原始值，再更改
        /// </summary>
        /// <param name="dbContext"></param>
        public static async Task<int> saveChangesAsyncClientWin(this DbContext dbContext, int retryCount = 1)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    return await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    retryCount--;
                    if (retryCount == -1)
                    {
                        throw ex;
                    }
                    saveFailed = true;
                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    //if( entry.State==EntityState.Deleted)
                    //{
                    //    var values = entry.GetDatabaseValues();
                    //}
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
            return -1;
        }
        /// <summary>
        /// 保存数据，客户端程序优先
        /// 并发冲突后，将数据库值设置成原始值，再更改
        /// </summary>
        /// <param name="dbContext"></param>
        public static int saveChangesClientWin(this DbContext dbContext, int retryCount = 1)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    return dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    retryCount--;
                    if (retryCount == -1)
                    {
                        throw ex;
                    }
                    saveFailed = true;
                    // Update original values from the database
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
            return -1;
        }

        #endregion
    }
}
