using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebAPI;
using ZM.Extensions.JsonConvertEx;
/// <summary>
/// 解析http请求信息
/// </summary>
public class HttpContextAnalysisMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpContextAnalysisMiddleware> _logger;

    private readonly HttpContextAnalysisOptions _options;
    public HttpContextAnalysisMiddleware(
        RequestDelegate next,
        ILogger<HttpContextAnalysisMiddleware> logger,
        HttpContextAnalysisOptions options
        )
    {
        _options = options;
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _options.Analysis(context);
        
        await _next.Invoke(context);
    }


}