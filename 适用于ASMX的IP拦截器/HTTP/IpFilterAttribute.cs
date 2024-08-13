using System;
using System.Configuration;

namespace WebForms.Filter.HTTP
{
    // HTTP协议调用ASMX的拦截器
    /******************************************************************************** 
	** 作者： 刘光智 
	** 创始时间：2024-06-28 
	** 描述：  
	**  
	*********************************************************************************/
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class IpFilterAttribute : Attribute
    {
        public string[] AllowedIps { get; private set; }

        public IpFilterAttribute(params string[] ips)
        {
            string configIps = ConfigurationManager.AppSettings["IpWhiteList"];
            var configAllowedIps = !string.IsNullOrEmpty(configIps)
                ? configIps.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : new string[] { };
            AllowedIps = new string[ips.Length + configAllowedIps.Length];
            ips.CopyTo(AllowedIps, 0);
            configAllowedIps.CopyTo(AllowedIps, ips.Length);
        }

        public bool IsAllowedIp(string userIp)
        {
            foreach (var ip in AllowedIps)
            {
                if (userIp == ip)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
