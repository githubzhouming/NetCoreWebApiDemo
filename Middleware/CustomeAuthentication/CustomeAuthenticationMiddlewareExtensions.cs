using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.DBContexts;
using Microsoft.Extensions.Configuration;
namespace WebAPI
{
    public static class CustomeAuthenticationMiddlewareExtensions
    {
        /// <summary>
        /// 使用请求路径检查
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomeAuthenticationMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomeAuthenticationMiddleware>();
        }
        /// <summary>
        /// 注册 请求路径检查
        /// </summary>
        /// <param name="services"></param>
        public static void AddCustomeAuthentication(this IServiceCollection services)
        {
            var iServiceProvider = services.BuildServiceProvider();
            var dbContext = iServiceProvider.GetService<EntityContext>();
            var cache = iServiceProvider.GetService<IDistributedCache>();
            var _configuration = iServiceProvider.GetService<IConfiguration>();
            string aesKey = _configuration.GetValue<string>("AESKey");
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
            CustomeAuthenticationOptions options = new CustomeAuthenticationOptions(dbContext, cache, aesKey);

            services.AddSingleton<CustomeAuthenticationOptions>(options);
        }
    }
}