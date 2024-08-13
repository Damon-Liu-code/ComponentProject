using System;
using System.Web.Services.Protocols;

namespace WebForms.Filter.SOAP
{
    // SOAP协议调用ASMX的拦截器
    /******************************************************************************** 
	** 作者： 刘光智 
	** 创始时间：2024-06-28 
	** 描述：  
	**  
	*********************************************************************************/
    [AttributeUsage(AttributeTargets.Method)]
    public class IpFilterAttribute : SoapExtensionAttribute
    {
        public override Type ExtensionType => typeof(IpFilter);

        public override int Priority { get; set; }

        public string[] AllowedIps { get; private set; }

        public IpFilterAttribute(params string[] ips)
        {
            AllowedIps = ips.Length > 0 ? ips : null;
            Priority = 1; 
        }
    }
}
