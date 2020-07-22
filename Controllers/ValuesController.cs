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
using StackExchange.Redis.Extensions.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using ZM.Extensions.DataEx;
using System.Linq.Dynamic.Core;

namespace WebAPI.Controllers
{
    [Route("/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _conf;
        private readonly EntityContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly WeChatInfo _weChatInfo;
        private readonly IRedisCacheClient _iRedisCacheClient;

        private readonly RabbitMQInfo _rabbitMQInfo;

        IServiceCollection _services;
        public ValuesController(EntityContext context, IServiceCollection services, ILogger<ValuesController> logger)
        {
            _logger = logger;
            _cache = services.BuildServiceProvider().GetService<IDistributedCache>();
            _conf = services.BuildServiceProvider().GetService<IConfiguration>();
            _context = context;
            _env = services.BuildServiceProvider().GetService<IWebHostEnvironment>();
            _weChatInfo = services.BuildServiceProvider().GetService<WeChatInfo>();
            _iRedisCacheClient = services.BuildServiceProvider().GetService<IRedisCacheClient>();
            _rabbitMQInfo = services.BuildServiceProvider().GetService<RabbitMQInfo>();
            _services = services;

        }
        // GET api/values
        [HttpGet("env")]
        public ActionResult getEnv()
        {

            CustomResult customResult = new CustomResult();
            try
            {
                var s= _cache.GetString("BaiduTrans0230D4709BB002DDB2B63C3984700C67");
                customResult.resultCode = 0;
                customResult.resultBody = Environment.GetEnvironmentVariables(); ;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }

        [HttpGet("testdb")]
        public ActionResult testdb()
        {

            CustomResult customResult = new CustomResult();
            try
            {
                // using (var scope = _services.BuildServiceProvider().CreateScope())
                // {
                //     using (var entityContext = scope.ServiceProvider.GetService<EntityContext>())
                //     {
                //         Models.SysUser item = new Models.SysUser();
                //         item.SysUserId = new Guid("C344B5FB-E88E-46E2-BC23-95C3A7CE6DDA");
                //         item.Name = "test112233";
                //         var oldData = entityContext.Find<Models.SysUser>(item.getId());
                //         item.setObjectValue(ref oldData);

                //         var entityEntry = entityContext.Entry(oldData);

                //         entityContext.SaveChanges();
                //     }
                // }
                Models.SysUser item = new Models.SysUser();
                item.SysUserId = new Guid("C344B5FB-E88E-46E2-BC23-95C3A7CE6DDA");
                item.Name = "test1";
                var oldData = _context.Find<Models.SysUser>(item.getId());
                item.setObjectValue(ref oldData);

                var result1= _context.Set<Models.SysUser>().FromSqlInterpolated($"SELECT [s].[SysUserID], [s].[name] AS [Name] FROM [SysUser] AS [s] where [s].[SysUserID]='C344B5FB-E88E-46E2-BC23-95C3A7CE6DDA'")
                .Select("new(SysUserID as SysUserID, name as Name)").ToDynamicArray();

                var entityEntry = _context.Entry(oldData);
                var query = _context.Set<Models.SysUser>()
                .Where("SysUserID == @0 && Name.Contains(@1)", "C344B5FB-E88E-46E2-BC23-95C3A7CE6DDA","test")
                .OrderBy("name")
                .Select("new(SysUserID as SysUserID, name as Name)");
                var s=query.ToDynamicList().SerializeObject();
                var s2= s.DeserializeObject<IList<Models.SysUser>>();
                _context.SaveChanges();
                customResult.resultCode = 0;
                customResult.resultBody = s;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }


        [HttpGet("redistest")]
        public async Task<ActionResult> redistest(int? length)
        {

            CustomResult customResult = new CustomResult();
            try
            {
                var cnt = 0;
                var cntStr = await _iRedisCacheClient.Db0.GetAsync<string>("redistestCnt");
                if (!string.IsNullOrEmpty(cntStr)) { cnt = int.Parse(cntStr); }
                if (length == null) { length = 100; }
                for (int i = 0; i < length; i++)
                {
                    cnt++;
                    await _iRedisCacheClient.Db0.AddAsync("redistestCnt", cnt);
                    await _iRedisCacheClient.Db0.AddAsync("redistestkey" + cnt, "value" + cnt);
                }

                customResult.resultCode = 0;
                customResult.resultBody = cnt;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }
        [HttpGet("agrs")]
        public ActionResult getAgrs()
        {

            CustomResult customResult = new CustomResult();
            try
            {
                customResult.resultCode = 0;
                customResult.resultBody = Environment.GetCommandLineArgs();
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }

        [HttpGet("info")]
        public ActionResult getWebInfo()
        {

            CustomResult customResult = new CustomResult();
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("This is a webapi application created by ASP.Net Core 3.0.");
                stringBuilder.AppendLine("Use Microsoft.EntityFrameworkCore (SqlServer).");
                stringBuilder.AppendLine("Use Microsoft.Extensions.Caching.Distributed (Redis).");
                stringBuilder.AppendLine("use NLog.Web.AspNetCore.");
                stringBuilder.AppendLine("Main use Middleware(UseStaticFiles,UseCors).");
                stringBuilder.AppendLine("Custom asp.net core Middleware (IP White List,UserToken,Request path permission check).");
                customResult.resultCode = 0;
                customResult.resultBody = stringBuilder.ToString();
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }

        [HttpGet("wechat")]
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

        [HttpPost("rabbitmqtest")]
        public virtual ActionResult rabbitmqtest(RabbitMQInfo info)
        {
            CustomResult customResult = new CustomResult();
            try
            {

                if (string.IsNullOrEmpty(info.hostName))
                {
                    info.hostName = _rabbitMQInfo.hostName;
                }
                if (info.port == 0)
                {
                    info.port = _rabbitMQInfo.port;
                }
                if (string.IsNullOrEmpty(info.userName))
                {
                    info.userName = _rabbitMQInfo.userName;
                }
                if (string.IsNullOrEmpty(info.password))
                {
                    info.password = _rabbitMQInfo.password;
                }
                if (string.IsNullOrEmpty(info.virtualHost))
                {
                    info.virtualHost = _rabbitMQInfo.virtualHost;
                }
                if (string.IsNullOrEmpty(info.exchange))
                {
                    info.exchange = _rabbitMQInfo.exchange;
                }
                if (string.IsNullOrEmpty(info.queueName))
                {
                    info.queueName = _rabbitMQInfo.queueName;
                }
                if (string.IsNullOrEmpty(info.routingKey))
                {
                    info.routingKey = _rabbitMQInfo.routingKey;
                }
                if (string.IsNullOrEmpty(info.message))
                {
                    info.message = _rabbitMQInfo.message;
                }
                if (string.IsNullOrEmpty(info.bindingKey))
                {
                    info.bindingKey = _rabbitMQInfo.bindingKey;
                }
                var req = new RabbitMQSendTopic();
                var result = req.Run(info);

                customResult.resultCode = 0;
                customResult.resultBody = result;
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
