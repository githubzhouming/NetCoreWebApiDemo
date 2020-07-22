using Newtonsoft.Json;
namespace ZM.Extensions.JsonConvertEx
{
    public static class JsonClassExtensions
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObject(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 序列化忽略空值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObjectIgnoreNull(this object value)
        {
            var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(value,Formatting.Indented,jsonSetting);
        }
    }
}
