using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using ZM.Extensions.DateTimeEx;
using ZM.Extensions.IDistributedCacheEx;
using ZM.Extensions.SecurityCryptographyEx;
using Microsoft.EntityFrameworkCore;
using ZM.Extensions.HttpContextEx;
using WebAPI.Common;
using System.Threading.Tasks;

public class CustomeAuthenticationOptions
{
    private readonly IDistributedCache _cache;
    private readonly DbContext _dbContext;
    private readonly string _aesKey;

    private static readonly string _userIgnorePathKey = "UserIgnorePath";
    private static readonly string _userAction = "UserAction";
    public CustomeAuthenticationOptions(DbContext dbContext, IDistributedCache cache, string aesKey)
    {
        _cache = cache;
        _dbContext = dbContext;
        _aesKey = aesKey;
    }
    public virtual IEnumerable<string> getPatternList(HttpContext context)
    {
        var userToken = context.getUserToken();
        var userid = userToken.userid;
        var cacheStr = _cache.getUserString(userid, _userIgnorePathKey);
        if (cacheStr != null)
        {
            //缓存存在，直接取缓存数据
            return cacheStr.Split(',');
        }
        var result = WebAPI.Common.DataServices.getUserIgnorePatternList(_dbContext, userid).Select((a) => { return a.PatternList; });
        List<string> ls = new List<string>();
        foreach (var str in result)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var strArr = str.Split(',');
                foreach (var str2 in strArr)
                {
                    if (!ls.Contains(str2))
                    {
                        ls.Add(str2);
                    }
                }
            }

        }
        //缓存起来
        _cache.setUserStringExpired(userid, _userIgnorePathKey, string.Join(",", ls));
        return ls;
    }

    public virtual IEnumerable<string> getUserActionList(HttpContext context)
    {
        var userToken = context.getUserToken();
        var userid = userToken.userid;
        var cacheStr = _cache.getUserString(userid, _userAction);
        if (cacheStr != null)
        {
            //缓存存在，直接取缓存数据
            return cacheStr.Split(',');
        }
        var result = WebAPI.Common.DataServices.getUserActionList(_dbContext, userid).Select((a) => { return a.SysActionName; });

        //缓存起来
        _cache.setUserStringExpired(userid, _userAction, string.Join(",", result));
        return result;
    }

    public virtual bool ignorePath(HttpContext context)
    {
        string path = context.Request.Path.Value.ToLower();
        //静态文件类型
        string pattern = "\\.(html|htm|css|js|json|xml|txt|gif|png|jpg|jpeg|ico)($|\\?)";
        if (Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase))
        {
            return true;
        }
        var patternList = getPatternList(context);
        foreach (var pa in patternList)
        {
            if (Regex.IsMatch(path, pa, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
    public virtual bool checkAction(HttpContext context, out string result)
    {
        string path = context.Request.Path.Value.ToLower();
        string method = context.Request.Method.ToLower();
        result = $"{path},{method}";
        var actionList = getUserActionList(context);
        foreach (var pa in actionList)
        {
            if (Regex.IsMatch(path, pa, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    public async Task<int> createActionAccessInfo(string requestKey)
    {
        return await DataServices.createActionAccessInfo(_dbContext, requestKey);
    }
}