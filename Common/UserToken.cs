using System;
using ZM.Extensions.JsonConvertEx;

public class UserToken
{
    /// <summary>
    /// 用户系统id
    /// </summary>
    /// <value></value>
    public Guid userid { get; set; }
    /// <summary>
    /// 用户登录名
    /// </summary>
    /// <value></value>
    public string username { get; set; }
    /// <summary>
    /// timestamp 秒
    /// </summary>
    /// <value></value>
    public long? timestamp { get; set; }
    /// <summary>
    /// token的唯一值
    /// </summary>
    /// <value></value>
    public string tokekey { get; set; }
    /// <summary>
    /// 签名
    /// </summary>
    /// <value></value>
    public string sign{get;set;}

    public override string ToString()
    {
        return this.SerializeObject();
    }
    public static UserToken Parse(string userTokenStr){
        return userTokenStr.DeserializeObject<UserToken>();
    }
}