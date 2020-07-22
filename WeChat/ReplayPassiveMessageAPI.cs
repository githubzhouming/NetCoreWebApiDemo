using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZM.Extensions.DateTimeEx;

namespace ZM.WeChat
{
    public class ReplayPassiveMessageAPI
    {
        /// <summary>
        /// 回复文本消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RepayText(string toUserName, string fromUserName, string content)
        {
            return $@"<xml><ToUserName><![CDATA[{toUserName}]]></ToUserName>
            <FromUserName><![CDATA[{fromUserName}]]></FromUserName>
            <CreateTime>{DateTimeHelper.GetTimeStampSeconds()}</CreateTime>
            <MsgType><![CDATA[text]]></MsgType>
            <Content><![CDATA[{content}]]></Content></xml>";
        }

        /// <summary>
        /// 回复单图文消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public static string RepayNews(string toUserName, string fromUserName, WeChatNews news)
        {
            List<WeChatNews> newsList=new List<WeChatNews>();
            newsList.Add(news);
            return RepayNews(toUserName,fromUserName,newsList);
        }

        /// <summary>
        /// 回复多图文消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public static string RepayNews(string toUserName, string fromUserName, List<WeChatNews> news)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append( $@"<xml><ToUserName><![CDATA[{toUserName}]]></ToUserName>
            <FromUserName><![CDATA[{fromUserName}]]></FromUserName>
            <CreateTime>{DateTimeHelper.GetTimeStampSeconds()}</CreateTime>
            <MsgType><![CDATA[news]]></MsgType>
            <ArticleCount>{news.Count}</ArticleCount><Articles>");
            foreach (var c in news)
            {
                stringBuilder.Append($@"<item><Title><![CDATA[{c.title}]]></Title>
                <Description><![CDATA[{c.description}]]></Description>
                <PicUrl><![CDATA[{c.picurl}]]></PicUrl>
                <Url><![CDATA[{c.url}]]></Url>
                </item>");
            }
            stringBuilder.Append("</Articles></xml>");
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 回复图片消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="media_id">已经上传到微信服务器的图片media_id</param>
        /// <returns></returns>
        public static string ReplayImage(string toUserName, string fromUserName, string media_id)
        {
            return $@"<xml><ToUserName><![CDATA[{toUserName}]]></ToUserName>
            <FromUserName><![CDATA[{fromUserName}]]></FromUserName>
            <CreateTime>{DateTimeHelper.GetTimeStampSeconds()}</CreateTime>
            <MsgType><![CDATA[image]]></MsgType>
            <Image><MediaId><![CDATA[{media_id}]]></MediaId></Image></xml>";
        }
        /// <summary>
        /// 回复语音消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="media_id">已经上传到微信服务器的语音media_id</param>
        /// <returns></returns>
        public static string ReplayVoice(string toUserName, string fromUserName, string media_id)
        {
            return $@"<xml><ToUserName><![CDATA[{toUserName}]]></ToUserName>
            <FromUserName><![CDATA[{fromUserName}]]></FromUserName>
            <CreateTime>{DateTimeHelper.GetTimeStampSeconds()}</CreateTime>
            <MsgType><![CDATA[voice]]></MsgType>
            <Voice><MediaId><![CDATA[{media_id}]]></MediaId></Voice></xml>";
        }
        /// <summary>
        /// 回复视频消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="media_id">已经上传到微信服务器的视频media_id</param>
        /// <param name="title">视频标题</param>
        /// <param name="description">视频文字说明</param>
        /// <returns></returns>
        public static string ReplayVedio(string toUserName, string fromUserName, string media_id, string title, string description)
        {
            return $@"<xml><ToUserName><![CDATA[{toUserName}]]></ToUserName>
            <FromUserName><![CDATA[{fromUserName}]]></FromUserName>
            <CreateTime>{DateTimeHelper.GetTimeStampSeconds()}</CreateTime>
            <MsgType><![CDATA[video]]></MsgType>
            <Video><MediaId><![CDATA[{media_id}]]></MediaId>
            <Title><![CDATA[{title}]]></Title>
            <Description><![CDATA[{description}]]></Description></Video></xml>";
        }

        /// <summary>
        /// 回复音乐消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="title">音乐标题</param>
        /// <param name="description">音乐描述</param>
        /// <param name="musicUrl">音乐链接</param>
        /// <param name="hqMusicUrl">高质量音乐链接，WIFI环境优先使用该链接播放音乐</param>
        /// <param name="thumb_media_id">缩略图的媒体id，通过上传多媒体文件，得到的id</param>
        /// <returns></returns>
        public static string ReplayMusic(string toUserName, string fromUserName, string title, string description, string musicUrl, string hqMusicUrl, string thumb_media_id)
        {
            return $@"<xml><ToUserName><![CDATA[{toUserName}]]></ToUserName>
            <FromUserName><![CDATA[{fromUserName}]]></FromUserName>
            <CreateTime>{DateTimeHelper.GetTimeStampSeconds()}</CreateTime>
            <MsgType><![CDATA[music]]></MsgType>
            <Music>
            <Title><![CDATA[{title}]]></Title>
            <Description><![CDATA[{description}]]></Description>
            <MusicUrl><![CDATA[{musicUrl}]]></MusicUrl>
            <HQMusicUrl><![CDATA[{hqMusicUrl}]]></HQMusicUrl>
            <ThumbMediaId><![CDATA[{thumb_media_id}]]></ThumbMediaId>
            </Music></xml>";
        }
    }
}
