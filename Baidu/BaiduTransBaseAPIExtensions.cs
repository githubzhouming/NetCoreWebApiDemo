using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using ZM.Extensions.JsonConvertEx;
using ZM.Extensions.SecurityCryptographyEx;

namespace ZM.Baidu.Extensions.API
{
    public static class BaiduTransBaseAPIExtensions
    {
        private static readonly string baseUrl="http://api.fanyi.baidu.com";
        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="client"></param>
        /// <param name="appid"></param>
        /// <param name="secretKey"></param>
        /// <param name="q"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Translate(this HttpClient client,string appid,string secretKey, string q, string from,string to,out string msg)
        {
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            string sign = (appid + q + salt + secretKey).MD5Encrypt("x2");
            var pathUrl = $"{baseUrl}/api/trans/vip/translate?q={HttpUtility.UrlEncode(q)}&from={from}&to={to}&appid={appid}&salt={salt}&sign={sign}";
            var result = client.GetAsync(pathUrl).Result;
            msg=result.ToString();
            if (!result.IsSuccessStatusCode) return string.Empty;
            msg= result.Content.ReadAsStringAsync().Result;
            var dynamicResult = msg.DeserializeObject<dynamic>();
            var error_code= dynamicResult.error_code;
            if(error_code!=null){
                return string.Empty;
            }
            var results=dynamicResult.trans_result.ToObject<List<dynamic>>();;
            StringBuilder stringBuilder=new StringBuilder();
            if(results.Count==1){
                return (string)results[0].dst;
            }
            //否则
            var i=0;
            foreach(var res in results){
                i++;
                stringBuilder.AppendLine(i.ToString()).AppendLine((string)res.dst);
            }
            return stringBuilder.ToString();
        }
    }
}