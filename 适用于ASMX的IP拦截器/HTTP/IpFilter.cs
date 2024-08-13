using System;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebForms.Filter.HTTP
{
    // HTTP协议调用ASMX的拦截器
    /******************************************************************************** 
	** 作者： 刘光智 
	** 创始时间：2024-06-28 
	** 描述：  
	**  
	*********************************************************************************/
    public class IpFilter : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
        }

        private void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            if (context.Request.Path.Contains(".asmx"))
            {
                string userIp = context.Request.UserHostAddress;
                string methodName = context.Request.PathInfo.Replace("/", "");

                Type webServiceType = GetWebServiceType(context.Request.Path);
                if (webServiceType != null)
                {
                    MethodInfo methodInfo = webServiceType.GetMethod(methodName);
                    if (methodInfo != null)
                    {
                        var attribute = methodInfo.GetCustomAttribute<IpFilterAttribute>();
                        if (attribute != null && !attribute.IsAllowedIp(userIp))
                        {
                            context.Response.StatusCode = 403; 
                            context.Response.ContentType = "text/plain";
                            context.Response.Write("Access denied: Your IP address is not allowed.");
                            context.Response.End();
                        }
                    }
                }
            }
        }

        private Type GetWebServiceType(string path)
        {
            string serviceName = path.Split('/')[2].Split('.').First();
            string namespacePrefix = "WebForms.CAS";
            string typeName = $"{namespacePrefix}.{serviceName}";
            Type serviceType = Type.GetType(typeName);
            if (serviceType == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    serviceType = assembly.GetType(typeName);
                    if (serviceType != null)
                    {
                        break;
                    }
                }
            }
            return serviceType;
        }

        public void Dispose() { }
    }
}
