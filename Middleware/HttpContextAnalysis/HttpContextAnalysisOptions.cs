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

public class HttpContextAnalysisOptions
{
    private readonly IDistributedCache _cache;
    private readonly DbContext _dbContext;
    private readonly string _aesKey;

    public HttpContextAnalysisOptions(DbContext dbContext, IDistributedCache cache, string aesKey)
    {
        _cache = cache;
        _dbContext = dbContext;
        _aesKey = aesKey;
    }

    public void Analysis(HttpContext context)
    {
        var tokekey = context.Request.Query["tokekey"].FirstOrDefault();
        var token = _cache.getUserTokenString(tokekey);
        UserToken userToken;
        if (string.IsNullOrEmpty(token))
        {
            userToken = new UserToken();
        }
        else
        {
            userToken = UserToken.Parse(token.AesDecrypt(_aesKey));
        }
        context.setUserToken(userToken);
        context.setRequestKey(DateTimeHelper.GetTimeStampMilliseconds().ToString()+Guid.NewGuid().ToString("N"));
    }
    

}