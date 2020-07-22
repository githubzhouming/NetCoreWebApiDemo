using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebAPI.Auth;
using WebAPI.Common;
using WebAPI.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using WebAPI.DBContexts;
using Microsoft.Extensions.FileProviders;
using System.IO;
using NLog.Extensions.Logging;
using ZM.WeChat;
using ZM.Baidu;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace WebAPI
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private readonly ILogger _logger;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, ILogger<Startup> logger, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ///注册本身，可以在其它地方调用
            services.AddSingleton<IServiceCollection>(services);
            #region 配置读取绑定
            var rabbitMQInfo=new RabbitMQInfo(){
                hostName=_configuration.GetValue<string>("hostName"),
                port=_configuration.GetValue<int>("port"),
                userName=_configuration.GetValue<string>("userName"),
                password=_configuration.GetValue<string>("password"),
                virtualHost=_configuration.GetValue<string>("virtualHost"),
                exchange=_configuration.GetValue<string>("exchange"),
                queueName=_configuration.GetValue<string>("queueName"),
                routingKey=_configuration.GetValue<string>("routingKey"),
                message=_configuration.GetValue<string>("message"),
                bindingKey=_configuration.GetValue<string>("bindingKey"),
            };
            services.AddSingleton<RabbitMQInfo>(rabbitMQInfo);
            var weChatInfo = new WeChatInfo()
            {
                Token = _configuration.GetValue<string>("WeChatToken"),
                EncodingAESKey = _configuration.GetValue<string>("WeChatEncodingAESKey"),
                AppID = _configuration.GetValue<string>("WeChatAppID"),
                AppSecret = _configuration.GetValue<string>("WeChatAppSecret"),
            };
            // _configuration.GetSection("WeChatInfo").Bind(weChatInfo);
            services.AddSingleton<WeChatInfo>(weChatInfo);

            var baiduTransInfo = new BaiduTransInfo()
            {
                appID = _configuration.GetValue<string>("BaiduTransAppID"),
                secretKey = _configuration.GetValue<string>("BaiduTransSecretKey"),
            };
            services.AddSingleton<BaiduTransInfo>(baiduTransInfo);

            var RedisConnStr = _configuration.GetValue<string>("RedisStr");

            #endregion
            #region 数据库配置
            var sqlServerString = _configuration.GetValue<string>("SqlserverStr");
            // if(string.IsNullOrEmpty(sqlServerString)){
            //     sqlServerString = _configuration.GetConnectionString("MyDB");
            // }


            services.AddTransient<EntityContext>((iServiceProvider) =>
            {

                var dbContextOption = new DbContextOptions<DbContext>();
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddNLog(_configuration);
                });
                var dbContextOptionBuilder = new DbContextOptionsBuilder<DbContext>(dbContextOption)
                        .UseSqlServer(sqlServerString)
                        .UseLoggerFactory(loggerFactory);
                var _dbContext = new EntityContext(dbContextOptionBuilder.Options);
                return _dbContext;
            });
            // services.AddDbContext<EntityContext>(options =>
            // {
            //     options.UseSqlServer(connectionString)
            //     .UseLoggerFactory(loggerFactory);
            // });
            RightHelper.init(services);
            #endregion

            #region Redis Session


            
            //
            // services.AddDistributedRedisCache(options =>
            // {
            //     options.Configuration = RedisConnStr;
            //     options.InstanceName = _env.ApplicationName;
            // });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = RedisConnStr;
                options.InstanceName = _env.ApplicationName;
            });
            //添加Session并设置过期时长
            //Session 过期时长分钟

            // services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(sessionOutTime); });

            #endregion
            var RedisClusterListStr = _configuration.GetValue<string>("RedisClusterListStr");
            var RedisClusterPassword = _configuration.GetValue<string>("RedisClusterPassword");
            var RedisHostArr= RedisClusterListStr.Split(',');
            var Hosts = new List<RedisHost>();
            foreach(var hostStr in RedisHostArr){
                var arr= hostStr.Split(':');
                Hosts.Add(new RedisHost(){Host = arr[0], Port = int.Parse(arr[1])});
            }
            var redisConfiguration = new RedisConfiguration()
            {
                AbortOnConnectFail = true,
                KeyPrefix = _env.ApplicationName,
                Hosts = Hosts.ToArray(),
                AllowAdmin = true,
                ConnectTimeout = 3000,
                Database = 0,
                Ssl = false,
                Password = RedisClusterPassword,
                ServerEnumerationStrategy = new ServerEnumerationStrategy()
                {
                    Mode = ServerEnumerationStrategy.ModeOptions.All,
                    TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                    UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
                }
            };
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);
            
            // StackExchange.Redis.Extensions.Core.Abstractions.IRedisCacheClient a =null;
            #region 跨域配置
            var coresUrls = _configuration.GetValue<string>("CoresUrls");
            // if(string.IsNullOrEmpty(coresUrls)){
            //     coresUrls = _configuration["AllowCoresUrls"];
            // }
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(coresUrls.Split(','))
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                });
            });
            #endregion

            #region JWT 一般验证配置



            // services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // })
            // .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            // {
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidIssuer = jwtSettings.Issuer,//Issuer
            //         ValidAudience = jwtSettings.Audience,//Audience
            //         //SecurityKey
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            //         ValidateIssuer = true, //Whether or not validate Issuer
            //         ValidateAudience = true, //Whether or not validate Audience
            //         ValidateLifetime = true, //Whether or not validate Failure time
            //         ValidateIssuerSigningKey = true, //Whether or not validate SecurityKey                   　　　　　　　　　　　
            //         ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkew)//Allowed server time offset

            //         //--------------TokenValidationParameters的参数默认值
            //         // RequireSignedTokens = true,
            //         // SaveSigninToken = false,
            //         // ValidateActor = false,
            //         // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
            //         // ValidateAudience = true,
            //         // ValidateIssuer = true, 
            //         // ValidateIssuerSigningKey = false,
            //         // 是否要求Token的Claims中必须包含Expires
            //         // RequireExpirationTime = true,
            //         // 允许的服务器时间偏移量
            //         // ClockSkew = TimeSpan.FromSeconds(300),
            //         // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
            //         // ValidateLifetime = true
            //     };
            //     options.Events = new JwtBearerEventsCustome() { };
            // });

            #endregion

            #region cookie 身份验证
            // services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            // .AddCookie(options =>
            // {
            //     options.EventsType = typeof(CustomCookieAuthenticationEvents);
            // });

            // services.AddScoped<CustomCookieAuthenticationEvents>();
            #endregion
            //services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(30);
            //   options.ExcludedHosts.Add("localhost");
            //   options.ExcludedHosts.Add("127.0.0.1");
            //});
            #region 分析token信息
            {
                services.AddHttpContextAnalysis();
            }
            #endregion
            #region IP检查
            {
                services.AddIIPCheckOptions();
            }
            #endregion

            #region 自定义权限检查 url
            {
                services.AddCustomeAuthentication();
            }
            #endregion
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IDistributedCache cache)
        {
            cache.SetStringAsync("env.ApplicationName", env.ApplicationName);
            cache.SetStringAsync("env.ContentRootFileProvider", env.ContentRootFileProvider.ToString());
            cache.SetStringAsync("env.ContentRootPath", env.ContentRootPath);
            cache.SetStringAsync("env.EnvironmentName", env.EnvironmentName);
            cache.SetStringAsync("env.WebRootPath", env.WebRootPath);
            cache.SetStringAsync("env.WebRootFileProvider", env.WebRootFileProvider.ToString());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
                app.UseExceptionHandler("/Error");
            }
            // app.Use(async (context, next) =>
            // {
            //     await next.Invoke();
            //     // Do logging or other work that doesn't write to the Response.
            // });

            app.UseHttpContextAnalysisMiddleware();
            app.UseIPCheckMiddleware();

            // app.UseDefaultFiles();必须在UseStaticFiles 前
            {
                DefaultFilesOptions options = new DefaultFilesOptions();
                options.DefaultFileNames.Clear();
                options.DefaultFileNames.Add("index.html");
                app.UseDefaultFiles(options);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                    ,
                RequestPath = ""
            });
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);



            app.UseCustomeAuthenticationMiddleware();
            #region 身份验证
            // app.UseAuthentication();
            #endregion
            // app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



            // app.UseStaticFiles();
            // app.UseSession();
            // app.UseHttpsRedirection();
            //app.UseMvc();
        }
    }
}
