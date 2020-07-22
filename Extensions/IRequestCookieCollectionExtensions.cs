using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ZM.Extensions.IResponseCookiesEx
{
    public static class IRequestCookieCollectionExtensions
    {
        /// <summary>
        /// 根据键获取对应的cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(this IRequestCookieCollection cookies, string key)
        {
            var value = "";
            cookies.TryGetValue(key, out value);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }
            return value;
        }
    }
}
