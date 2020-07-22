using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebAPI.DBContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ZM.Baidu;
using ZM.Baidu.Extensions.API;
using ZM.Extensions.DateTimeEx;
using ZM.Extensions.DataEx;
using ZM.Extensions.HttpContextEx;
using System.Linq;
using System.Net.Http;
using ZM.Extensions.SecurityCryptographyEx;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BaiduTransBaseAPIController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _conf;
        private readonly EntityContext _context;
        private readonly BaiduTransInfo _baiduTransInfo;
        public BaiduTransBaseAPIController(EntityContext context, IServiceCollection services, ILogger<ValuesController> logger)
        {
            _logger = logger;
            _cache = services.BuildServiceProvider().GetService<IDistributedCache>();
            _conf = services.BuildServiceProvider().GetService<IConfiguration>();
            _context = context;
            _baiduTransInfo = services.BuildServiceProvider().GetService<BaiduTransInfo>();
        }
        private string getTranslate(string q, string from, string to, out string msg)
        {
            var catchstr = "BaiduTrans"+(q + from + to).MD5Encrypt();
            var result = _cache.GetString(catchstr);
            msg = string.Empty;
            if (string.IsNullOrEmpty(result))
            {
                HttpClient weChatClient = new HttpClient();
                result = weChatClient.Translate(_baiduTransInfo.appID, _baiduTransInfo.secretKey, q, from, to, out msg);
                if (!string.IsNullOrEmpty(result))
                {
                    _cache.SetString(catchstr, result);
                }
            }
            return result;
        }

        [HttpGet("translate")]
        public ActionResult GetTranslate(string q, string from, string to)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                string msg = string.Empty;
                var result = getTranslate(q, from, to, out msg);
                if (string.IsNullOrEmpty(result))
                {
                    customResult.resultCode = ResultCodeEnum.Bad;
                    customResult.resultBody = msg;
                    return BadRequest(customResult);
                }

                {
                    customResult.resultCode = 0;
                    customResult.resultBody = result;
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
