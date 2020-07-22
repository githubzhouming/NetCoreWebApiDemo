using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.CustomModels
{
    public class SqlAdvancedSelect
    {
        /// <summary>
        /// 查询的字段，表.字段
        /// </summary>
        public List<string> fields { get; set; }
        /// <summary>
        /// 主表
        /// </summary>
        public string mainTable { get; set; }
        /// <summary>
        /// 连接语句
        /// </summary>
        public string joinStr { get; set; }
        public string whereStr { get; set; }
        public string groupStr { get; set; }
        public string orderStr { get; set; }
        /// <summary>
        /// 起始数据位置,优先使用
        /// </summary>
        public int startIndex { get; set; }
        /// <summary>
        /// 结束数据位置,优先使用
        /// </summary>
        public int endIndex { get; set; }
        /// <summary>
        /// 每页数据量
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 查看第几页
        /// </summary>
        public int pageIndex { get; set; }
        public string getSql(string otherWhere)
        {
            if (pageSize == 0) { pageSize = 10; }
            if (startIndex == 0 && endIndex == 0)
            {
                if (pageIndex != 0)
                {
                    startIndex = pageIndex * pageSize;
                }
                endIndex = startIndex + pageSize;
            }
            var fieldsStr = $"[{mainTable}].*";
            if (fields != null && fields.Count > 0)
            {
                var separator = $"],[{mainTable}].[";
                fieldsStr = $"[{mainTable}].[" + string.Join(separator, fields) + "]";
            }

            if (string.IsNullOrEmpty(orderStr))
            {
                if (fields != null && fields.Count > 0)
                {
                    orderStr = fieldsStr;
                }else
                {
                    orderStr = "1";
                }
                
            }
            if (!WebAPI.Common.SQlCheckHelper.CheckSqlStr(fieldsStr))
            {
                fieldsStr = $"[{mainTable}].*";
            }
            if (!WebAPI.Common.SQlCheckHelper.CheckSqlStr(orderStr))
            {
                orderStr = "1";
            }
            if (!WebAPI.Common.SQlCheckHelper.CheckSqlStr(joinStr))
            {
                joinStr = string.Empty;
            }
            if (!WebAPI.Common.SQlCheckHelper.CheckSqlStr(whereStr))
            {
                whereStr = "1=2";
            }
            if (!WebAPI.Common.SQlCheckHelper.CheckSqlStr(otherWhere))
            {
                otherWhere = "1=2";
            }

            var sql = $@"select * from(
select top {endIndex} {fieldsStr},ROW_NUMBER()over(order by {orderStr}) rownum
from[{mainTable}] with(nolock) {joinStr} where (({whereStr}) and ({otherWhere})) order by {orderStr})a
where a.rownum>{startIndex}
order by a.rownum";
            return sql;
        }
    }
}
