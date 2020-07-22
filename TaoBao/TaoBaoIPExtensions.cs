using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using ZM.Extensions.JsonConvertEx;
using ZM.Extensions.SecurityCryptographyEx;

namespace ZM.TaoBao.Extensions.IP
{
    public static class TaoBaoIPExtensions
    {
        private static readonly string baseUrl="http://ip.taobao.com";
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
        public static dynamic IpTranslate(this HttpClient client,string ip,out string msg)
        {
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            var pathUrl = $"{baseUrl}/service/getIpInfo.php?ip={ip}&salt={salt}";
            var result = client.GetAsync(pathUrl).Result;
            msg=result.ToString();
            if (!result.IsSuccessStatusCode) return string.Empty;
            msg= result.Content.ReadAsStringAsync().Result;
            var dynamicResult = msg.DeserializeObject<dynamic>();
            var error_code= dynamicResult.code;
            if(error_code!=0){
                return string.Empty;
            }
            var results=dynamicResult.data.ToObject<dynamic>();
            
            return results;
        }
    }
}