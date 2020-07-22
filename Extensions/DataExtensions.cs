using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZM.Extensions.DataEx
{
    /// <summary>
    /// 数据操作
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// byte[] 转字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="strFormat"></param>
        /// <returns></returns>
        public static string BytesToString(this byte[] bytes, string strFormat = "X2")
        {
            StringBuilder sb = new StringBuilder(40);
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString(strFormat));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 字符串转成数据流
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Stream ToStream(this string str, ref long length)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                    writer.Flush();
                    length = stream.Length;
                    return stream;
                }
            }
        }
        /// <summary>
        /// DataTable 转集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IEnumerable<T> ToEnumerable<T>(this DataTable dt) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                ts[i] = t;
                i++;
            }
            return ts;
        }

        /// <summary>
        /// 简单对象更新值，值为 default(T) 的不更新
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <param name="tragetObj"></param>
        public static void setObjectValue<T>(this T sourceObj, ref T targetObj, IDictionary<string, object> defaultValueDic, bool ignoreNull = true) where T : class
        {
            var sourcePropertys = sourceObj.GetType().GetProperties();
            var targetPropertys = targetObj.GetType().GetProperties();
            foreach (var sourceProperty in sourcePropertys)
            {
                var targetProperty = targetPropertys.Where((a) => a.Name == sourceProperty.Name).FirstOrDefault();
                var sourceData = sourceProperty.GetValue(sourceObj);
                var targetData = targetProperty == null ? null : targetProperty.GetValue(targetObj);
                if (sourceData == default(T))
                {//如果是默认值不覆盖
                var keyStr=sourceProperty.Name.ToUpper();
                    if (defaultValueDic != null && defaultValueDic.ContainsKey(keyStr))
                    {
                        var obj = defaultValueDic[keyStr];
                        Func<object> objFun = obj as Func<object>;
                        object value = default(T);
                        if (objFun != null)
                        {
                            value = objFun();
                        }
                        else if (obj != null)
                        {
                            value = obj;
                        }
                        sourceData = value;
                    }
                }


                if (ignoreNull && sourceData == default(T))
                {
                    continue;
                }
                if (sourceData == default(T) && targetData == default(T))
                {
                    continue;
                }
                if ((sourceData != null && sourceData.Equals(targetData))
                || (targetData != null && targetData.Equals(sourceData))
                )
                {
                    continue;
                }


                targetProperty.SetValue(targetObj, sourceData);
            }
            var sourceFields = sourceObj.GetType().GetFields();
            var targetFields = targetObj.GetType().GetFields();
            foreach (var sourceField in sourceFields)
            {
                var targetField = targetFields.Where((a) => a.Name == sourceField.Name).FirstOrDefault();
                var sourceData = sourceField.GetValue(sourceObj);
                var targetData = targetField == null ? null : targetField.GetValue(targetObj);
                if (sourceData == default(T))
                {//如果是默认值不覆盖
                var keyStr=sourceField.Name.ToUpper();
                    if (defaultValueDic != null && defaultValueDic.ContainsKey(keyStr))
                    {
                        var obj = defaultValueDic[keyStr];
                        Func<object> objFun = obj as Func<object>;
                        object value = default(T);
                        if (objFun != null)
                        {
                            value = objFun();
                        }
                        else if (obj != null)
                        {
                            value = obj;
                        }
                        sourceData = value;
                    }
                }

                if (ignoreNull && sourceData == default(T))
                {
                    continue;
                }
                if (sourceData == default(T) && targetData == default(T))
                {
                    continue;
                }
                if ((sourceData != null && sourceData.Equals(targetData))
                || (targetData != null && targetData.Equals(sourceData))
                )
                {
                    continue;
                }

                targetField.SetValue(targetObj, sourceData);
            }
        }
        private static IDictionary<string, object> defaultValueDic = new Dictionary<string, object>();
        private static IDictionary<string, object> getDefualtValue()
        {
            if (defaultValueDic.Count == 0)
            {
                Func<object> func = () => DateTime.Now;
                defaultValueDic.Add("ModifiedOn".ToUpper(), func);
            }
            return defaultValueDic;
        }
        public static void setObjectValue<T>(this T sourceObj, ref T targetObj, bool ignoreNull = true) where T : class
        {
            var dic = getDefualtValue();
            sourceObj.setObjectValue(ref targetObj, dic, ignoreNull);
        }

        /// <summary>
        /// 生成单个随机数字
        /// </summary>
        private static int createNum()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int num = random.Next(10);
            return num;
        }

        /// <summary>
        /// 生成单个大写随机字母
        /// </summary>
        private static string createBigAbc()
        {
            //A-Z的 ASCII值为65-90
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int num = random.Next(65, 91);
            string abc = Convert.ToChar(num).ToString();
            return abc;
        }

        /// <summary>
        /// 生成单个小写随机字母
        /// </summary>
        private static string createSmallAbc()
        {
            //a-z的 ASCII值为97-122
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int num = random.Next(97, 123);
            string abc = Convert.ToChar(num).ToString();
            return abc;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">字符串的长度</param>
        /// <returns></returns>
        public static string CreateRandomStr(int length)
        {
            // 创建一个StringBuilder对象存储密码
            StringBuilder sb = new StringBuilder();
            //使用for循环把单个字符填充进StringBuilder对象里面变成14位密码字符串
            for (int i = 0; i < length; i++)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                //随机选择里面其中的一种字符生成
                switch (random.Next(3))
                {
                    case 0:
                        //调用生成生成随机数字的方法
                        sb.Append(createNum());
                        break;
                    case 1:
                        //调用生成生成随机小写字母的方法
                        sb.Append(createSmallAbc());
                        break;
                    case 2:
                        //调用生成生成随机大写字母的方法
                        sb.Append(createBigAbc());
                        break;
                }
            }
            return sb.ToString();
        }

    }
}
