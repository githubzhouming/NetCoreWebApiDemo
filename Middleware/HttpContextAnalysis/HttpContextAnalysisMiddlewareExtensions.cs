using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.DBContexts;
using Microsoft.Extensions.Configuration;
using ZM.Extensions.IConfigurationEx;
namespace WebAPI
{
    public static class HttpContextAnalysisMiddlewareExtensions
    {
        /// <summary>
        /// 解析HttpContex
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpContextAnalysisMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpContextAnalysisMiddleware>();
        }
        /// <summary>
        /// 注册分析token
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpContextAnalysis(this IServiceCollection services){
            var iServiceProvider= services.BuildServiceProvider();
            var dbContext=iServiceProvider.GetService<EntityContext>();
            var cache=iServiceProvider.GetService<IDistributedCache>();
            var _configuration=iServiceProvider.GetService<IConfiguration>();
            string aesKey=_configuration.getToeknAESKey();
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }
            if (cache is null)
            {
                throw new ArgumentNullException(nameof(cache));
            }
            if (string.IsNullOrEmpty(aesKey))
            {
                throw new ArgumentNullException("aesKey");
            }
            HttpContextAnalysisOptions options=new HttpContextAnalysisOptions(dbContext,cache,aesKey);

            services.AddSingleton<HttpContextAnalysisOptions>(options);
        }
    }
}