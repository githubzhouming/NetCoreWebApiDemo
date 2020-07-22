using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using ZM.Extensions.SecurityCryptographyEx;

namespace ZM.WeChat
{
    public class WeChatHelper
    {
        public static bool CheckSignature(string signature, string timestamp, string nonce, string token)
        {
            var arr = new[] { token, timestamp, nonce }.OrderBy(z => z).ToArray();
            var arrString = string.Join("", arr);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
            StringBuilder enText = new StringBuilder();
            foreach (var b in sha1Arr)
            {
                enText.AppendFormat("{0:x2}", b);
            }
            return signature == enText.ToString();
        }

        public static string GetJSApiSign(string jsapi_ticket, string noncestr, long timestamp, string url, out string string1)
        {
            var string1Builder = new StringBuilder();
            string1Builder.Append("jsapi_ticket=").Append(jsapi_ticket).Append("&")
                          .Append("noncestr=").Append(noncestr).Append("&")
                          .Append("timestamp=").Append(timestamp).Append("&")
                          .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            string1 = string1Builder.ToString();
            return string1.SHA1Encrypt("x2");
        }

        public static WeChatMessage Parse(string message)
        {
            var msg = new WeChatMessage();
            msg.Body = new DynamicXml(message);
            string msgType = msg.Body.MsgType.Value;
            switch (msgType)
            {
                case "text":
                    msg.Type = WeChatMessageType.Text;
                    break;
                case "image":
                    msg.Type = WeChatMessageType.Image;
                    break;
                case "voice":
                    msg.Type = WeChatMessageType.Voice;
                    break;
                case "video":
                    msg.Type = WeChatMessageType.Video;
                    break;
                case "location":
                    msg.Type = WeChatMessageType.Location;
                    break;
                case "link":
                    msg.Type = WeChatMessageType.Link;
                    break;
                case "event":
                    msg.Type = WeChatMessageType.Event;
                    break;
                default: throw new Exception("does not support this message type:" + msgType);
            }
            return msg;
        }
    }
}