using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using ZM.Extensions.JsonConvertEx;

namespace ZM.WeChat.Extensions.API
{
    public static class WeChatAPIExtensions
    {
        public static string GetAccessToken(this WeChatClient client,string appid, string secrect,out string msg)
        {
            var pathUrl = $"{WeChatClient.BaseUrl}/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={secrect}";
            var result = client.GetAsync(pathUrl).Result;
            msg=result.ToString();
            if (!result.IsSuccessStatusCode) return string.Empty;
            msg= result.Content.ReadAsStringAsync().Result;
            var dic = msg.DeserializeObject<IDictionary<string,object>>();
            if(dic.ContainsKey("access_token")){
                return dic["access_token"].ToString();
            }
            return string.Empty;
        }

        public static string GetJSApiTicket(this WeChatClient client,string access_token,out string msg)
        {
            var pathUrl = $"{WeChatClient.BaseUrl}/cgi-bin/ticket/getticket?access_token={access_token}&type=jsapi";
            var result = client.GetAsync(pathUrl).Result;
            msg=result.ToString();
            if (!result.IsSuccessStatusCode) return string.Empty;
            msg= result.Content.ReadAsStringAsync().Result;
            var dic = msg.DeserializeObject<IDictionary<string,object>>();
            if(dic.ContainsKey("ticket")){
                return dic["ticket"].ToString();
            }
            return string.Empty;
        }
    }
}