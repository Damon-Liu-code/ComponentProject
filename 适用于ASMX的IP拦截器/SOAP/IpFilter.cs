using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
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
    public class IpFilter : SoapExtension
    {
        private Stream oldStream;
        private Stream newStream;
        private string[] allowedIps;

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            var ipFilterAttribute = attribute as IpFilterAttribute;
            return ipFilterAttribute?.AllowedIps;
        }

        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {
            allowedIps = initializer as string[];
        }

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.AfterSerialize:
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    CheckIpValidation(message);
                    break;
                case SoapMessageStage.AfterDeserialize:
                    break;
            }
        }

        private void CheckIpValidation(SoapMessage message)
        {
            try
            {
                string clientIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(clientIp))
                {
                    clientIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (!IsValidIp(clientIp))
                {
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.StatusCode = 403;
                    HttpContext.Current.Response.ContentType = "text/plain";
                    HttpContext.Current.Response.Write("Access denied: Your IP address is not allowed.");
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception ex)
            {
                WebForms.App_Code.log4Helper.Error(this.GetType(), "Exception in CheckIpValidation: " + ex.Message);
                throw;
            }
        }

        private bool IsValidIp(string ip)
        {
            string configIps = ConfigurationManager.AppSettings["IpWhiteList"];
            string[] configAllowedIps = !string.IsNullOrWhiteSpace(configIps)
                ? configIps.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : new string[] { };
            var allAllowedIps = (allowedIps == null ? new string[] { } : allowedIps).Concat(configAllowedIps).ToArray();
            bool result = allAllowedIps.Any(allowIp => ip == allowIp);
            return result;
        }

    }
}
