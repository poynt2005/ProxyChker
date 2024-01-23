using ProxyChker.NET.DataDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyChker;

namespace ProxyChker.NET
{

    public partial class Form1 : Form
    {
        private ProxyChkerCore? ProxyChkerCoreInstance { get; set; } = null;

        private ResponseProxyChkerProxyUrl ProcessProxyGetUrl()
        {
            ResponseProxyChkerProxyUrl result = new();

            try
            {
                ProxyChkerCoreInstance ??= new();
                ProxyChkerCoreInstance.ReadProxy();

                result.ProxyUrl = ProxyChkerCoreInstance.LastProxyUrl;
                result.IsSuccess = true;
            }
            catch(Exception ex) 
            { 
                result.ErrorReason = ex.ToString();
            }

            return result;
        }

        private ResponseProxyChkerProxyCheckResult ProcessChkerCheck()
        {
            ResponseProxyChkerProxyCheckResult response = new();

            if(ProxyChkerCoreInstance == null)
            {
                response.ErrorReason = "proxychker instance is not initialized yet";
                return response;
            }

            try
            {
                ProxyChkerCoreInstance.ChkProxyAlive();
                response.IsProxyAlive = ProxyChkerCoreInstance.LastProxyIsAlive;
                response.IsSuccess = true;
            }
            catch(Exception ex) 
            { 
                response.ErrorReason = ex.ToString();
            }

            return response;
        }
    }
}
