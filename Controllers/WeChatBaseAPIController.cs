using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebAPI.DBContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ZM.WeChat;
using ZM.WeChat.Extensions.API;
using ZM.Extensions.DateTimeEx;
using ZM.Extensions.DataEx;
using ZM.Extensions.HttpContextEx;
using System.Linq;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WeChatBaseAPIController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _conf;
        private readonly EntityContext _context;
        private readonly WeChatInfo _weChatInfo;
        public WeChatBaseAPIController(EntityContext context, IServiceCollection services, ILogger<ValuesController> logger)
        {
            _logger = logger;
            _cache = services.BuildServiceProvider().GetService<IDistributedCache>();
            _conf = services.BuildServiceProvider().GetService<IConfiguration>();
            _context = context;
            _weChatInfo = services.BuildServiceProvider().GetService<WeChatInfo>();
        }

        private string getAccesstoken(out string msg)
        {
            var accessToken = _cache.GetString("accesstoken");
            msg = string.Empty;
            if (string.IsNullOrEmpty(accessToken))
            {
                WeChatClient weChatClient = new WeChatClient();
                accessToken = weChatClient.GetAccessToken(_weChatInfo.AppID, _weChatInfo.AppSecret, out msg);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    _cache.SetString("accesstoken", accessToken);
                }
            }
            return accessToken;
        }

        [HttpGet("accesstoken")]
        public ActionResult GetAccesstoken()
        {
            CustomResult customResult = new CustomResult();
            try
            {
                string msg = string.Empty;
                var accessToken = getAccesstoken(out msg);
                if (string.IsNullOrEmpty(accessToken))
                {
                    customResult.resultCode = ResultCodeEnum.Bad;
                    customResult.resultBody = msg;
                    return BadRequest(customResult);
                }

                {
                    customResult.resultCode = 0;
                    customResult.resultBody = accessToken;
                }

                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }

        private string getJSApiTicket(out string msg)
        {
            msg = string.Empty;
            var jsapi_ticket = _cache.GetString("jsapi_ticket");
            var accesstoken = getAccesstoken(out msg);
            if (string.IsNullOrEmpty(jsapi_ticket))
            {
                WeChatClient weChatClient = new WeChatClient();
                jsapi_ticket = weChatClient.GetJSApiTicket(accesstoken, out msg);
                if (!string.IsNullOrEmpty(jsapi_ticket))
                {
                    _cache.SetString("jsapi_ticket", jsapi_ticket);
                }
            }
            return jsapi_ticket;
        }
        [HttpGet("ticket")]
        public ActionResult GetJSApiTicket()
        {
            CustomResult customResult = new CustomResult();
            try
            {
                var msg = string.Empty;
                var jsapi_ticket = getJSApiTicket(out msg);
                if (string.IsNullOrEmpty(jsapi_ticket))
                {
                    customResult.resultCode = ResultCodeEnum.Bad;
                    customResult.resultBody = msg;
                    return BadRequest(customResult);
                }

                {
                    customResult.resultCode = 0;
                    customResult.resultBody = jsapi_ticket;
                }

                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }
        [HttpGet("jsapiinfo")]
        public ActionResult GetJSApiInfo()
        {
            CustomResult customResult = new CustomResult();
            try
            {
                var msg = string.Empty;
                var jsapi_ticket = getJSApiTicket(out msg);
                if (string.IsNullOrEmpty(jsapi_ticket))
                {
                    customResult.resultCode = ResultCodeEnum.Bad;
                    customResult.resultBody = msg;
                    return BadRequest(customResult);
                }
                var nonceStr = DataExtensions.CreateRandomStr(10);
                var timestamp = DateTimeHelper.GetTimeStampSeconds();
                var url = this.HttpContext.Request.Query["thisurl"].FirstOrDefault();
                if(string.IsNullOrEmpty(url)){
                    url = this.HttpContext.Request.GetAbsoluteUri();
                }
                
                var string1 = string.Empty;
                var signature = WeChatHelper.GetJSApiSign(jsapi_ticket, nonceStr, timestamp, url, out string1);
                {
                    customResult.resultCode = 0;
                    customResult.resultBody = new
                    {
                        appId = _weChatInfo.AppID,
                        timestamp = DateTimeHelper.GetTimeStampSeconds(),
                        nonceStr = DataExtensions.CreateRandomStr(10),
                        signature=signature
                    };
                }

                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }
    }
}
