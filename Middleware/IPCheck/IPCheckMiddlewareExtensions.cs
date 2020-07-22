using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.DBContexts;
using Microsoft.Extensions.Configuration;

namespace WebAPI
{
    public static class IPCheckMiddlewareExtensions
    {
        /// <summary>
        /// 使用IP检查
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIPCheckMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IPCheckMiddleware>();
        }
        /// <summary>
        /// 注册IP检查
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddIIPCheckOptions(this IServiceCollection services){
            var iServiceProvider= services.BuildServiceProvider();
            var dbContext=iServiceProvider.GetService<EntityContext>();
            var cache=iServiceProvider.GetService<IDistributedCache>();
            var _configuration=iServiceProvider.GetService<IConfiguration>();
            string aesKey=_configuration.GetValue<string>("AESKey");
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

            IPCheckOptions options=new IPCheckOptions(dbContext,cache,aesKey);

            services.AddSingleton<IPCheckOptions>(options);

        }
    }
}