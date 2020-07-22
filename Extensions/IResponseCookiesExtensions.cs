using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ZM.Extensions.IResponseCookiesEx
{
    public static class IResponseCookiesExtensions
    {
        /// <summary>
        /// 添加cookie缓存不设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddCookie(this IResponseCookies cookies, string key, string value)
        {
            cookies.Append(key, value);
        }
        /// <summary>
        /// 添加cookie缓存设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time">秒</param>
        public static void AddCookie(this IResponseCookies cookies,string key, string value, int time)
        {
            cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(time)
            });
        }
        /// <summary>
        /// 删除cookie缓存
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteCookie(this IResponseCookies cookies,string key)
        {
            cookies.Delete(key);
        }
    }
}
