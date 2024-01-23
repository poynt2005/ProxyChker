using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using ProxyChker.NET.DataDefs;

namespace ProxyChker.NET;

public partial class Form1 : Form
{
    private static readonly string StageURL = "http://127.0.0.1:3000";
    private string WebviewURL { get; }

    private static readonly string[] SupportedWebviewRemoteCallEvents = { "MessageBox", "ProxyChker" };

    private ProxyChkerCore? ProxyChkerCore { get; set; } = null;

    public Form1(string webviewURL)
    {
        InitializeComponent();
        WebviewURL = webviewURL;

        Icon = Resource1.favicon;
    }

    public Form1() : this(StageURL) { }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await WaitCoreWebViewLoad();

    }

    private void WebviewMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var requestWrapper = JsonConvert.DeserializeObject<RequestWrapper>(e.WebMessageAsJson);
        var targetSplitted = requestWrapper.Target.Split('_');

        if (!SupportedWebviewRemoteCallEvents.Contains(targetSplitted[0]))
        {
            ResponseBase response = new()
            {
                Target = requestWrapper.Target,
                ErrorReason = $"target call event: {targetSplitted[0]} is not supported"
            };
            webviewComponent.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            return;
        }

        BeginInvoke(async () =>
        {
            ResponseBase response = new();

            switch (targetSplitted[0])
            {
                case "MessageBox":
                    {
                        var req = requestWrapper.HtmlRequest.ToObject<RequestMessageBox>();
                        response = ProcessMessageBox(targetSplitted[1], req.Title, req.Message);
                        break;
                    }
                case "ProxyChker":
                    {
                        switch(targetSplitted[1])
                        {
                            case "GetUrl": 
                                {
                                    var req = requestWrapper.HtmlRequest.ToObject<RequestProxyChkerGetProxyUrl>();
                                    response = ProcessProxyGetUrl();
                                    break;
                                }
                            case "Chk":
                                {
                                    var req = requestWrapper.HtmlRequest.ToObject<RequestProxyChkerCheckProxyAlive>();
                                    response = ProcessChkerCheck();
                                    break;
                                }
                        }
                        break;
                    }
            }

            response.Target = requestWrapper.Target;
            webviewComponent.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
        });
    }

    private async Task WaitCoreWebViewLoad()
    {
        var webviewEnvironment = await CoreWebView2Environment.CreateAsync(null, "./UserData", null);

        await webviewComponent.EnsureCoreWebView2Async(webviewEnvironment);

        webviewComponent.WebMessageReceived += WebviewMessageReceived;
        webviewComponent.CoreWebView2.Settings.IsPinchZoomEnabled = false;
        webviewComponent.CoreWebView2.Settings.IsZoomControlEnabled = false;
        webviewComponent.CoreWebView2.Settings.AreDevToolsEnabled = false;
        webviewComponent.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        webviewComponent.CoreWebView2.Navigate(WebviewURL);
    }
}
