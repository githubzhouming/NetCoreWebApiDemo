using System;
// using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// using Microsoft.IdentityModel.Tokens;
using WebAPI.DBContexts;
using WebAPI.Models;
using ZM.Extensions.DateTimeEx;
using ZM.Extensions.IDistributedCacheEx;
using ZM.Extensions.SecurityCryptographyEx;
using ZM.Extensions.IConfigurationEx;
namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly DbContext _context;
        private readonly ILogger<TokenController> _logger;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _conf;
        public TokenController(EntityContext context, IServiceCollection services, ILogger<TokenController> logger)
        {
            _context = context;
            _logger = logger;
            _cache = services.BuildServiceProvider().GetService<IDistributedCache>();
            _conf = services.BuildServiceProvider().GetService<IConfiguration>();
        }
        /*
        [HttpPost("jwt/getToken")]
        public virtual async Task<IActionResult> getJWTToken(UserToken item)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                if (string.IsNullOrEmpty(item.username))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The username is empty";
                    return BadRequest(customResult);
                }
                if (item.timestamp == null)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The timestamp is empty";
                    return BadRequest(customResult);
                }
                if (string.IsNullOrEmpty(item.sign))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The sign is empty";
                    return BadRequest(customResult);
                }
                var datetime1 = DateTimeHelper.ConvertTimestampSeconds(item.timestamp.Value);
                var dateNow = DateTime.Now;
                if (datetime1 < dateNow.AddMinutes(-5) || datetime1 > dateNow.AddMinutes(5))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The timestamp over time";
                    return BadRequest(customResult);
                }
                //var users = await _context.Set<SysUser>().FromSql($"SELECT * FROM dbo.SysUser where name={item.name} and password={item.password} and 1={1}").ToArrayAsync();
                var users = await _context.Set<SysUser>().Where(a => a.Name == item.username).ToArrayAsync();

                if (users == null || users.Length == 0)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "username does not exist";
                    return BadRequest(customResult);
                }
                if (users.Length > 1)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "username repetition";
                    return BadRequest(customResult);
                }
                var authInfo = users.First();
                if ((item.username + authInfo.Password + item.timestamp).MD5Encrypt() != item.sign)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "sign verification failed";
                    return BadRequest(customResult);
                }


                var claims = new Claim[]{
                    new Claim(ClaimTypes.Sid,authInfo.SysUserId.ToString())
                };
                ;
                //对称秘钥
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                //签名证书(秘钥，加密算法)
                var creds = new SigningCredentials(key, _jwtSettings.Algorithm);

                //生成token  [注意]需要nuget添加Microsoft.AspNetCore.Authentication.JwtBearer包，并引用System.IdentityModel.Tokens.Jwt命名空间
                var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer, audience: _jwtSettings.Audience, claims: claims, notBefore: DateTime.Now, expires: DateTime.Now.AddSeconds(_jwtSettings.Expires), signingCredentials: creds);

                customResult.resultCode = 0;
                customResult.resultBody = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(customResult);

            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }
         */
        [HttpPost("login/getToken")]
        public virtual async Task<IActionResult> getLoginToken(UserToken item)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                if (string.IsNullOrEmpty(item.username))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The username is empty";
                    return BadRequest(customResult);
                }
                if (item.timestamp == null)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The timestamp is empty";
                    return BadRequest(customResult);
                }
                if (string.IsNullOrEmpty(item.sign))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The sign is empty";
                    return BadRequest(customResult);
                }
                var datetime1 = DateTimeHelper.ConvertTimestampSeconds(item.timestamp.Value);
                var dateNow = DateTime.Now;
                if (datetime1 < dateNow.AddMinutes(-5) || datetime1 > dateNow.AddMinutes(5))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The timestamp over time";
                    return BadRequest(customResult);
                }
                //var users = await _context.Set<SysUser>().FromSql($"SELECT * FROM dbo.SysUser where name={item.name} and password={item.password} and 1={1}").ToArrayAsync();
                var users = await _context.Set<SysUser>().Where(a => a.Name == item.username).ToArrayAsync();

                if (users == null || users.Length == 0)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "username does not exist";
                    return BadRequest(customResult);
                }
                if (users.Length > 1)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "username repetition";
                    return BadRequest(customResult);
                }
                var authInfo = users.First();
                if ((item.username + authInfo.Password + item.timestamp).MD5Encrypt() != item.sign)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "sign verification failed";
                    return BadRequest(customResult);
                }
                var tokekey = _cache.getUserTokenkeyCacheString(authInfo.SysUserId);
                var token = _cache.getUserTokenString(tokekey);
                UserToken userToken=null;
                var AESKey = _conf.getToeknAESKey();
                if (string.IsNullOrEmpty(token))
                {
                    userToken = new UserToken()
                    {
                        userid = authInfo.SysUserId,
                        username = authInfo.Name,
                        timestamp = DateTimeHelper.GetTimeStampSeconds(),
                    };
                    tokekey= userToken.tokekey = userToken.ToString().MD5Encrypt();

                    token = userToken.ToString().AesEncrypt(AESKey);
                }else
                {
                    userToken = UserToken.Parse(token.AesDecrypt(AESKey));
                }
                
                //缓存信息
                _cache.setUserTokenStringExpired(tokekey, token);
                _cache.setUserTokenkeyStringExpired(userToken.userid, tokekey);
                customResult.resultCode = 0;
                customResult.resultBody =new {tokekey=tokekey,userid=userToken.userid,username=item.username} ;
                return Ok(customResult);

            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }
        [HttpPost("renewal/getToken")]
        public virtual ActionResult<IActionResult> getRenewalToken(UserToken item)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                if (item.userid == Guid.Empty)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The username is empty";
                    return BadRequest(customResult);
                }
                if (item.timestamp == null)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The timestamp is empty";
                    return BadRequest(customResult);
                }
                if (string.IsNullOrEmpty(item.sign))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The sign is empty";
                    return BadRequest(customResult);
                }
                var datetime1 = DateTimeHelper.ConvertTimestampSeconds(item.timestamp.Value);
                var dateNow = DateTime.Now;
                if (datetime1 < dateNow.AddMinutes(-5) || datetime1 > dateNow.AddMinutes(5))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The timestamp over time";
                    return BadRequest(customResult);
                }
                var tokekey = _cache.getUserTokenkeyCacheString(item.userid);
                if (string.IsNullOrEmpty(tokekey))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The userid is invalid";
                    return BadRequest(customResult);
                }
                var token = _cache.getUserTokenString(tokekey);
                if (string.IsNullOrEmpty(tokekey))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "The tokekey is invalid";
                    return BadRequest(customResult);
                }
                var AESKey = _conf.getToeknAESKey();
                var userToken = UserToken.Parse(token.AesDecrypt(AESKey));
                if ((userToken.userid + userToken.tokekey + item.timestamp).MD5Encrypt() != item.sign)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "sign verification failed";
                    return BadRequest(customResult);
                }

                //缓存信息
                _cache.setUserTokenStringExpired(tokekey, token);
                _cache.setUserTokenkeyStringExpired(item.userid, tokekey);
                customResult.resultCode = 0;
                customResult.resultBody = tokekey;
                return Ok(customResult);

            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }
    }
}