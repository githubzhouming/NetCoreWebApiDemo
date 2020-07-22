using System;
using System.Net.Http;
using System.Text;

namespace ZM.WeChat
{
    public class WeChatClient : HttpClient
    {
        public static readonly string BaseUrl="https://api.weixin.qq.com";
        public WeChatClient()
        {
            
        }
    }
}