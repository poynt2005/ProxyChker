using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChker.NET.DataDefs
{
    public class RequestWrapper
    {
        public string Target { get; set; } = String.Empty;
        public JObject HtmlRequest { get; set; } = new();
    }

    public class RequestBase
    {

    }

    public class RequestProxyChkerGetProxyUrl: RequestBase
    {

    }

    public class RequestProxyChkerCheckProxyAlive: RequestBase
    {

    }

    public class RequestMessageBox : RequestBase
    {
        public string Message { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;
    }

    public class ResponseBase
    {
        public string Target { get; set; } = String.Empty;
        public bool IsSuccess { get; set; } = false;
        public string ErrorReason { get; set; } = String.Empty;
    }

    public class ResponseMessageBox : ResponseBase
    {
        public bool IsConfirm { get; set; } = true;
    }

    public class ResponseProxyChkerProxyUrl: ResponseBase
    {
        public string ProxyUrl { get; set; } = String.Empty;
    }

    public class ResponseProxyChkerProxyCheckResult: ResponseBase
    {
        public bool IsProxyAlive { get; set; } = false;
    }
}