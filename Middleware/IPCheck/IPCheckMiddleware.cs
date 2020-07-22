using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using WebAPI;
using ZM.Extensions.JsonConvertEx;
using Microsoft.AspNetCore.HttpOverrides;
using ZM.Extensions.HttpContextEx;
using System.Text;
/// <summary>
/// Ip地址检查
/// </summary>
public class IPCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IPCheckMiddleware> _logger;
    private readonly IPCheckOptions _options;

    public IPCheckMiddleware(
        RequestDelegate next,
        ILogger<IPCheckMiddleware> logger,
        IPCheckOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task Invoke(HttpContext context)
    {
        string url=context.Request.GetAbsoluteUri();
        string path = context.Request.Path.ToString().ToLower();
        string method = context.Request.Method.ToLower();
        string ip = GetClientIp(context);
        var userToken = context.getUserToken();
        var requestKey=context.getRequestKey();


        _logger.LogDebug($@"Request from Head{getHead(context)}");
        _logger.LogDebug($"Request from Remote IP address: {ip}");
        var isBlocked= !_options.CheakIp(context,ip);
        await _options.createAccessInfo(userToken.userid,url,path,ip,method,isBlocked,requestKey);
        if (isBlocked)
        {
            CustomResult customResult = new CustomResult();
            customResult.resultCode = ResultCodeEnum.InvalidIP;
            customResult.resultBody = $"URL is not on the whitelist: {ip}";
            _logger.LogDebug($"URL is not on the whitelist: {ip}");
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(customResult.SerializeObject());
            return;
        }

        await _next.Invoke(context);
    }
    private static StringBuilder getHead(HttpContext context){
        StringBuilder stringBuilder=new StringBuilder();
        foreach(var head in context.Request.Headers){
            stringBuilder.AppendLine($"{head.Key}:{head.Value}");
        }
        return stringBuilder;
    }
    private static string GetClientIp(HttpContext context)
    {
        //如果一个 HTTP 请求到达服务器之前，经过了三个代理 Proxy1、Proxy2、Proxy3
        //，IP 分别为 IP1、IP2、IP3，用户真实 IP 为 IP0
        //结果为 X-Forwarded-For: IP0, IP1, IP2
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if(!string.IsNullOrEmpty(ip)){
            if(ip.IndexOf(",")!=-1){
                var spArr= ip.Replace(" ",string.Empty).Split(',');
                ip=spArr[0];
            }
        }
        if (string.IsNullOrEmpty(ip))
        {
            //ip = context.Connection.RemoteIpAddress.ToString();
             ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
            // ip = context.Connection.RemoteIpAddress.MapToIPv6().ToString();
        }
        return ip;
    }
    
}