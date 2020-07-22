using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZM.WeChat
{
    public enum WeChatMessageType
    {
        Text, //文本
        Location, //地理位置
        Image, //图片
        Voice, //语音
        Video, //视频
        Link, //连接信息
        Event, //事件推送
    }
    public class WeChatMessage
    {
        public virtual WeChatMessageType Type { set; get; }
        public virtual dynamic Body { set; get; }
    }


}
