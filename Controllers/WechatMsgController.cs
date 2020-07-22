using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebAPI.DBContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ZM.Extensions.JsonConvertEx;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using ZM.WeChat;
using System.IO;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WeChatMsgController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _conf;
        private readonly EntityContext _context;
        private readonly WeChatInfo _weChatInfo;
        public WeChatMsgController(EntityContext context, IServiceCollection services, ILogger<ValuesController> logger)
        {
            _logger = logger;
            _cache = services.BuildServiceProvider().GetService<IDistributedCache>();
            _conf = services.BuildServiceProvider().GetService<IConfiguration>();
            _context = context;
            _weChatInfo = services.BuildServiceProvider().GetService<WeChatInfo>();
        }
        [HttpGet("")]
        public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        {
            var token = _weChatInfo.Token;//微信公众平台后台设置的Token
            if (string.IsNullOrEmpty(token)) return Content("请先设置Token！");
            if (!WeChatHelper.CheckSignature(signature, timestamp, nonce, token))
            {
                return Content("参数错误！");
            }
            return Content(echostr); //返回随机字符串则表示验证通过
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// </summary>
        [HttpPost("")]
        public async Task<ActionResult> Post(string signature, string timestamp, string nonce, string echostr)
        {
            var request = HttpContext.Request;
            //默认不支持多次读取请求体
            Request.EnableBuffering();
            var safeMode = request.Query["encrypt_type"].FirstOrDefault() == "aes";
            WeChatMessage message = null;
            using (var streamReader = new StreamReader(request.Body))
            {
                var decryptMsg = string.Empty;
                var msg = await streamReader.ReadToEndAsync();
                request.Body.Position = 0;
                #region 解密
                if (safeMode)
                {
                    var msg_signature = request.Query["msg_signature"].FirstOrDefault();
                    var WeChatBizMsgCrypt = new WeChatBizMsgCrypt(_weChatInfo.Token, _weChatInfo.EncodingAESKey, _weChatInfo.AppID);
                    var ret = WeChatBizMsgCrypt.DecryptMsg(msg_signature, timestamp, nonce, msg, ref decryptMsg);
                    if (ret != 0)//解密失败
                    {
                        //TODO：开发者解密失败的业务处理逻辑
                        //注意：本demo用log4net记录此信息，你可以用其他方法
                        _logger.LogError($"decrypt message return {ret}, request body {msg}");
                    }
                }
                else
                {
                    decryptMsg = msg;
                }
                #endregion
                _logger.LogTrace(decryptMsg);
                message = WeChatHelper.Parse(decryptMsg);
            }
            var response = new WeChatMsgOptions().Execute(message);
            var encryptMsg = string.Empty;

            #region 加密
            if (safeMode)
            {
                var msg_signature = request.Query["msg_signature"];
                var WeChatBizMsgCrypt = new WeChatBizMsgCrypt(_weChatInfo.Token, _weChatInfo.EncodingAESKey, _weChatInfo.AppID);
                var ret = WeChatBizMsgCrypt.EncryptMsg(response, timestamp, nonce, ref encryptMsg);
                if (ret != 0)//加密失败
                {
                    //TODO：开发者加密失败的业务处理逻辑
                    _logger.LogError(string.Format("encrypt message return {0}, response body {1}", ret, response));
                }
            }
            else
            {
                encryptMsg = response;
            }
            #endregion

            return new ContentResult
            {
                Content = encryptMsg,
                ContentType = "text/xml"
            };
        }

    }
}
