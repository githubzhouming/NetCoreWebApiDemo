using System;
using Microsoft.Extensions.Configuration;

namespace ZM.Extensions.IConfigurationEx
{
    public static class IConfigurationExtensions
    {
        public static string getToeknAESKey(this IConfiguration _configuration)
        {
            return _configuration.GetValue<string>("AESKey");
        }
    }
}
