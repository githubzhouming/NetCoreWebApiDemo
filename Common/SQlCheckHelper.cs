using System;
using System.Collections.Generic;
using System.Text;
namespace WebAPI.Common
{
    /// <summary>
    /// 检查是否包含关键字
    /// </summary>
    public static class SQlCheckHelper
    {
        private static readonly List<string> sqlKeyWordList = new List<string>() { "exec ", "exec[", "insert ", "insert[", "delete ", "delete[", "update ", "update[", "declare ", "declare[", "truncate ", "truncate[" };
        private static bool CheckSqlStr(string sqlStr, int type)
        {
            List<string> keyWordList;
            switch (type)
            {
                case 0:
                    keyWordList = sqlKeyWordList;
                    break;
                default:
                    keyWordList = sqlKeyWordList;
                    break;
            }
            if (!string.IsNullOrWhiteSpace(sqlStr))
            {
                sqlStr = sqlStr.ToLower();
                foreach (var keyStr in keyWordList)
                {
                    if (sqlStr.IndexOf(keyStr) >= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool CheckSqlStr(string sqlStr)
        {
            return CheckSqlStr(sqlStr, 0);
        }
    }
}
