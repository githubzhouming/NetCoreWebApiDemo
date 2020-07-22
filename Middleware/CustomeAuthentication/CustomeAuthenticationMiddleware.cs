using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebAPI;
using ZM.Extensions.JsonConvertEx;
using ZM.Extensions.HttpContextEx;
/// <summary>
/// 自定义权限检查
/// </summary>
public class CustomeAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomeAuthenticationMiddleware> _logger;

    private readonly CustomeAuthenticationOptions _options;
    public CustomeAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<CustomeAuthenticationMiddleware> logger,
        CustomeAuthenticationOptions options
        )
    {
        _options = options;
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        string result;
        if (!_options.ignorePath(context))
        {
            if (!_options.checkAction(context, out result))
            {
                var requestKey=context.getRequestKey();
                await _options.createActionAccessInfo(requestKey);
                CustomResult customResult = new CustomResult();
                customResult.resultCode = ResultCodeEnum.InvalidAuthAction;
                customResult.resultBody = result;
                _logger.LogDebug(result);
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(customResult.SerializeObject());
                return;
            }
        }


        await _next.Invoke(context);
    }


}