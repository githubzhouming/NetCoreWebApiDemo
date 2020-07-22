using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebAPI.Common;
using ZM.Extensions.IDistributedCacheEx;
using ZM.Extensions.HttpContextEx;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ZM.TaoBao.Extensions.IP;
using System.Net.Http;
using System.Text;

public class IPCheckOptions
{
    private readonly DbContext _dbContext;
    private readonly IDistributedCache _cache;
    private readonly string _aesKey;
    private static readonly string _IpWhiteListIpKey = "IpWhiteListIp";
    private static readonly string _IpWhiteListNameKey = "IpWhiteListName";
    private static readonly string _IpNameRefKey = "IpNameRef";
    private static readonly string _IpNameRef2Key = "IpName2Ref";
    private static readonly string _IpWhiteListCheckKey = "IpWhiteListCheckKey";
    public IPCheckOptions(DbContext dbContext, IDistributedCache cache, string aesKey)
    {
        _cache = cache;
        _dbContext = dbContext;
        _aesKey = aesKey;
    }

    private bool isMatch(string value, IEnumerable<string> strList)
    {
        foreach (var pattern in strList)
        {
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    public virtual bool CheakIp(HttpContext context, string ip)
    {
        //ip不存在直接返回false
        if (string.IsNullOrEmpty(ip))
        {
            return false;
        }
        var userToken = context.getUserToken();
        var userid = userToken.userid;
        //取出缓存的结果
        var cacheCheckStr = _cache.getUserString(userid, _IpWhiteListCheckKey + ip);
        if (!string.IsNullOrEmpty(cacheCheckStr))
        {
            return "1" == cacheCheckStr;
        }


        var cacheIpStr = _cache.getUserString(userid, _IpWhiteListIpKey);
        var cacheNameStr = _cache.getUserString(userid, _IpWhiteListNameKey);
        List<string> ipls = null;
        List<string> namels = null;
        if (cacheIpStr == null || cacheNameStr == null)
        {
            var ipWhitelist = DataServices.getUserIPWhiteList(_dbContext, userid);
            ipls = new List<string>();
            namels = new List<string>();
            foreach (var ipWhite in ipWhitelist)
            {
                if (!string.IsNullOrEmpty(ipWhite.UrlList))
                {
                    var strArr = ipWhite.UrlList.Split(',');
                    foreach (var str in strArr)
                    {
                        if (!ipls.Contains(str))
                        {
                            ipls.Add(str);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ipWhite.NameList))
                {
                    var str2Arr = ipWhite.NameList.Split(',');
                    foreach (var str in str2Arr)
                    {
                        if (!namels.Contains(str))
                        {
                            namels.Add(str);
                        }
                    }
                }

            }
            //缓存起来
            _cache.setUserStringExpired(userid, _IpWhiteListIpKey, string.Join(",", ipls));
            _cache.setUserStringExpired(userid, _IpWhiteListNameKey, string.Join(",", namels));
        }
        else
        {
            ipls = cacheIpStr.Split(',').ToList();
            namels = cacheNameStr.Split(',').ToList();
        }
        if (isMatch(ip, ipls))
        {
            _cache.setUserStringExpired(userid, _IpWhiteListCheckKey + ip, "1");
            return true;
        }

        //通过数据库getIpNameRef
        var cacheIpName2RefStr = _cache.GetString(_IpNameRef2Key + ip);
        if (string.IsNullOrEmpty(cacheIpName2RefStr))
        {
            var dataArr = ip.Split('.');
            if (dataArr.Length != 4)
            {
                return false;
            }
            var ipValue = 256 * 256 * 256 * long.Parse(dataArr[0]) + 256 * 256 * long.Parse(dataArr[1]) + 256 * long.Parse(dataArr[2]) + long.Parse(dataArr[1]);
            var IpNameRefList = DataServices.getIpNameRef(_dbContext, ipValue);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var obj in IpNameRefList)
            {
                stringBuilder.Append($"{obj.Country},{obj.Local}");
            }
            cacheIpName2RefStr=stringBuilder.ToString();
            _cache.SetString(_IpNameRef2Key + ip, cacheIpName2RefStr);
        }
        if (!string.IsNullOrEmpty(cacheIpName2RefStr)&& isMatch(cacheIpName2RefStr, namels)){
            _cache.setUserStringExpired(userid, _IpWhiteListCheckKey + ip, "1");
            return true;
        }


        //通过淘宝ip转换得出地址名称判断
        var cacheIpNameRefStr = _cache.GetString(_IpNameRefKey + ip);
        if (string.IsNullOrEmpty(cacheIpNameRefStr))
        {
            var clint = new HttpClient();
            var ipInfo = clint.IpTranslate(ip, out string msg);
            if ((ipInfo as string) == string.Empty)
            {
                //ip解析失败，直接返回false
                return false;
            }
            var country = (string)ipInfo.country;
            var region = (string)ipInfo.region;
            var city = (string)ipInfo.city;
            var county = (string)ipInfo.county;
            var area = (string)ipInfo.area;
            var isp = (string)ipInfo.isp;
            cacheIpNameRefStr = $"{country},{region},{city},{county},{area},{isp}";
            _cache.setString(_IpNameRefKey + ip, cacheIpNameRefStr, TimeSpan.FromDays(7));
        }
        if ((!string.IsNullOrEmpty(cacheIpNameRefStr)) && isMatch(cacheIpNameRefStr, namels))
        {
            _cache.setUserStringExpired(userid, _IpWhiteListCheckKey + ip, "1");
            return true;
        }
        _cache.setUserStringExpired(userid, _IpWhiteListCheckKey + ip, "0");
        return false;
    }

    public async Task<int> createAccessInfo(Guid userid, string Url, string Path, string Ip, string Method, bool IsBlocked, string requestKey)
    {
        return await DataServices.createAccessInfo(_dbContext, userid, Url, Path, Ip, Method, IsBlocked, requestKey);
    }
}