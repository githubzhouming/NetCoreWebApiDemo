using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
namespace ZM.Extensions.HttpContextEx
{
    public static class HttpContextExtensions
    {
        private static readonly string userTokeKey="userTokeKey"+Guid.NewGuid().ToString();
        private static readonly string requestKey="requestKey"+Guid.NewGuid().ToString();
        public static void setUserToken(this HttpContext context,UserToken value){
            var contextItems=context.Items;
            if(contextItems.ContainsKey(userTokeKey)){
                contextItems[userTokeKey]=value;
            }else{
                contextItems.Add(userTokeKey,value);
            }
        }

        public static UserToken getUserToken(this HttpContext context){
            var contextItems=context.Items;
            if(contextItems.ContainsKey(userTokeKey)){
                return (UserToken)contextItems[userTokeKey];
            }
            return null;
        }

        public static void setRequestKey(this HttpContext context,string key){
            var contextItems=context.Items;
            if(contextItems.ContainsKey(requestKey)){
                contextItems[requestKey]=key;
            }else{
                contextItems.Add(requestKey,key);
            }
        }
        public static string getRequestKey(this HttpContext context){
            var contextItems=context.Items;
            if(contextItems.ContainsKey(requestKey)){
                return (string)contextItems[requestKey];
            }
            return null;
        }

        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }
    }
}