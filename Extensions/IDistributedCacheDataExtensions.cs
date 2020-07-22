using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace ZM.Extensions.IDistributedCacheEx
{
    /// <summary>
    /// 数据操作
    /// </summary>
    public static class IDistributedCacheDataExtensions
    {
        private static Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions defaultOptions = setDefaultOptions();
        private static DistributedCacheEntryOptions setDefaultOptions()
        {
            var options = new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return options;
        }
        private static DistributedCacheEntryOptions TimeSpanToDefaultOptions(TimeSpan timeSpan){
            var options = new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = timeSpan;
            return options;
        }
        private static string getUserKey(object userid,string key){
            return userid+key;
        }

        public static async Task setStringAsync(this IDistributedCache cache, string key, string value,TimeSpan timeSpan)
        {
            await cache.SetStringAsync(key, value,TimeSpanToDefaultOptions(timeSpan));
        }
        public static void setString(this IDistributedCache cache, string key, string value,TimeSpan timeSpan)
        {
             cache.SetString(key, value,TimeSpanToDefaultOptions(timeSpan));
        }

        public static async Task<string> getUserStringAsync(this IDistributedCache cache, object userid, string key)
        {
            var value = await cache.GetStringAsync(getUserKey(userid,key));
            return value;
        }
        public static async Task setUserStringAsync(this IDistributedCache cache, object userid, string key, string value)
        {
            await cache.SetStringAsync(getUserKey(userid,key), value);
        }
        public static async Task setUserStringAsync(this IDistributedCache cache, object userid, string key, string value,DistributedCacheEntryOptions options)
        {
            await cache.SetStringAsync(getUserKey(userid,key), value,options);
        }
        public static async Task setUserStringAsync(this IDistributedCache cache, object userid, string key, string value,TimeSpan timeSpan)
        {
            await cache.setUserStringAsync(userid,key, value,TimeSpanToDefaultOptions(timeSpan));
        }

        public static async Task setUserStringAsyncExpired(this IDistributedCache cache, object userid, string key, string value,DistributedCacheEntryOptions options=null)
        {
            if(options==null){options=defaultOptions;}
            await cache.setUserStringAsync(userid,key, value, options);
        }
        public static async Task setUserStringAsyncExpired(this IDistributedCache cache, object userid, string key, string value,TimeSpan timeSpan)
        {
            await cache.setUserStringAsync(userid,key,value,timeSpan);
        }


        public static string getUserString(this IDistributedCache cache, object userid, string key)
        {
            var value = cache.GetString(getUserKey(userid,key));
            return value;
        }
        public static void setUserString(this IDistributedCache cache, object userid, string key, string value)
        {
            cache.SetString(getUserKey(userid,key), value);
        }
        public static void setUserString(this IDistributedCache cache, object userid, string key, string value,DistributedCacheEntryOptions options)
        {
            cache.SetString(getUserKey(userid,key), value, options);
        }
        public static void setUserString(this IDistributedCache cache, object userid, string key, string value,TimeSpan timeSpan)
        {
            cache.setUserString(userid,key, value, TimeSpanToDefaultOptions(timeSpan));
        }
        public static void setUserStringExpired(this IDistributedCache cache, object userid, string key, string value,DistributedCacheEntryOptions options=null)
        {
            if(options==null){options=defaultOptions;}
            cache.setUserString(userid , key, value, defaultOptions);
        }
        public static void setUserStringExpired(this IDistributedCache cache, object userid, string key, string value,TimeSpan timeSpan)
        {
            cache.setUserString(userid ,key, value, timeSpan);
        }


        #region 业务逻辑用
        private static readonly string userTokenCacheKey="userTokenCacheKey";
        private static readonly string userTokenkeyCacheKey="userTokenkeyCacheKey";
        public static void setUserTokenString(this IDistributedCache cache, object key, string value)
        {
            cache.setUserString(key,userTokenCacheKey, value);
        }
        public static void setUserTokenString(this IDistributedCache cache, object key, string value,DistributedCacheEntryOptions options)
        {
            cache.setUserString(key,userTokenCacheKey, value,options);
        }
        public static void setUserTokenString(this IDistributedCache cache, object key, string value,TimeSpan timeSpan)
        {
            cache.setUserString(key,userTokenCacheKey, value,timeSpan);
        }
        /// <summary>
        /// 设置usertoken信息
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">tokenkey</param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public static void setUserTokenStringExpired(this IDistributedCache cache, object key, string value,DistributedCacheEntryOptions options=null)
        {
            cache.setUserStringExpired(key,userTokenCacheKey, value, options);
        }
        public static void setUserTokenStringExpired(this IDistributedCache cache, object key, string value,TimeSpan timeSpan)
        {
            cache.setUserStringExpired(key,userTokenCacheKey, value, timeSpan);
        }
        /// <summary>
        /// 获取usertoken信息
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">tokekey</param>
        /// <returns></returns>
        public static string getUserTokenString(this IDistributedCache cache, object key)
        {
            var value = cache.getUserString(key, userTokenCacheKey);
            return value;
        }
        public static void setUserTokenkeyString(this IDistributedCache cache, object key, string value)
        {
            cache.setUserString(key,userTokenkeyCacheKey, value);
        }
        public static void setUserTokenkeyString(this IDistributedCache cache, object key, string value,DistributedCacheEntryOptions options)
        {
            cache.setUserString(key,userTokenkeyCacheKey, value, options);
        }
        public static void setUserTokenkeyString(this IDistributedCache cache, object key, string value,TimeSpan timeSpan)
        {
            cache.setUserString(key,userTokenkeyCacheKey, value, timeSpan);
        }
        /// <summary>
        /// 设置 tokekey 信息
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">userid</param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public static void setUserTokenkeyStringExpired(this IDistributedCache cache, object key, string value,DistributedCacheEntryOptions options=null)
        {
            cache.setUserStringExpired(key,userTokenkeyCacheKey, value, options);
        }
        public static void setUserTokenkeyStringExpired(this IDistributedCache cache, object key, string value,TimeSpan timeSpan)
        {
            cache.setUserStringExpired(key,userTokenkeyCacheKey, value, timeSpan);
        }
        /// <summary>
        /// 获取 tokekey 信息
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">userid</param>
        /// <returns></returns>
        public static string getUserTokenkeyCacheString(this IDistributedCache cache, object key)
        {
            var value = cache.getUserString(key, userTokenkeyCacheKey);
            return value;
        }
        
        #endregion
    }
}
