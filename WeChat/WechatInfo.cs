using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ZM.WeChat
{
    public class WeChatInfo
    {
        /// <summary>
        /// 令牌(Token)
        /// </summary>
        /// <value></value>
        public string Token { set; get; }
        /// <summary>
        /// 消息加解密密钥
        /// </summary>
        /// <value></value>
        public string EncodingAESKey { set; get; }
        /// <summary>
        /// 开发者ID
        /// </summary>
        /// <value></value>
        public string AppID { set; get; }
        /// <summary>
        /// 开发者密码
        /// </summary>
        /// <value></value>
        public string AppSecret { set; get; }
    }
}