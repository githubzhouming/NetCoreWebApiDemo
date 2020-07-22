using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZM.Extensions.JsonConvertEx;

namespace WebAPI
{
    /// <summary>
    /// 自定义webpai返回对象
    /// </summary>
    public class CustomResult
    {
        public ResultCodeEnum resultCode { get; set; }
        public object resultBody { get; set; }

        public override string ToString()
        {
            return this.SerializeObject();
        }
    }

    public enum ResultCodeEnum
    {
        OK=0,
        InvalidParameter=-1,
        Bad=-10,
        InvalidAuth=-60,
        InvalidAuthPath=-61,
        InvalidAuthToken=-61,
        InvalidAuthAction=-62,
        InvalidIP=-70,
        InvalidAction = -80,
        InvalidTableRight = -81,
        Exception =-90
    }
}
