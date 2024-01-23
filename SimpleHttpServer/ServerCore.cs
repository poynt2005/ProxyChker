using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using SimpleHttpServer.Lib;

namespace SimpleHttpServer;

public class ServerCore : IDisposable
{
    private HttpListener Listener { get; }

    private static readonly string Google404ErrorTemplate = @"<!DOCTYPE html> <html lang=en> <meta charset=utf-8> <title>Error 404 (Not Found)!!1</title> <style> *{margin:0;padding:0}html,code{font:15px/22px arial,sans-serif}html{background:#fff;color:#222;padding:15px}body{margin:7% auto 0;max-width:390px;min-height:180px;padding:30px 0 15px}* > body{background:url(//www.google.com/images/errors/robot.png) 100% 5px no-repeat;padding-right:205px}p{margin:11px 0 22px;overflow:hidden}ins{color:#777;text-decoration:none}a img{border:0}@media screen and (max-width:772px){body{background:none;margin-top:0;max-width:none;padding-right:0}}#logo{background:url(//www.google.com/images/branding/googlelogo/1x/googlelogo_color_150x54dp.png) no-repeat;margin-left:-5px}@media only screen and (min-resolution:192dpi){#logo{background:url(//www.google.com/images/branding/googlelogo/2x/googlelogo_color_150x54dp.png) no-repeat 0% 0%/100% 100%;-moz-border-image:url(//www.google.com/images/branding/googlelogo/2x/googlelogo_color_150x54dp.png) 0}}@media only screen and (-webkit-min-device-pixel-ratio:2){#logo{background:url(//www.google.com/images/branding/googlelogo/2x/googlelogo_color_150x54dp.png) no-repeat;-webkit-background-size:100% 100%}}#logo{display:inline-block;height:54px;width:150px} </style> <a href=//www.google.com/><span id=logo aria-label=Google></span></a> <p><b>404.</b> <ins>That's an error.</ins> <p>The requested URL <code>%RequestURL%</code> was not found on this server.  <ins>That's all we know.</ins>";

    private bool ServerRunning { get; set; } = false;
    private bool Disposed { get; set; } = false;

    public string BindAddress { get; private set; } = String.Empty;
    public int ListenerPort { get; private set; } = -1;

    private List<string> ServDirectories { get; } = [];

    public ServerCore() : this("127.0.0.1", GetRandomUnusedPort()) { }

    public ServerCore(string addr) : this(addr, GetRandomUnusedPort()) { }

    public ServerCore(string addr, int port)
    {
        Listener = new();
        BindAddress = $"http://{addr}:{port}/";
        Listener.Prefixes.Add(BindAddress);

        Listener.Start();
        ServerRunning = true;
    }

    public void AddServDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            throw new Exception($"target serve directory: {directory} is not exists");
        }

        if (!ServDirectories.Contains(directory))
        {
            ServDirectories.Add(directory);
        }
    }

    public async Task RunAsync()
    {
        await Serv();
    }

    private async Task Serv()
    {
        while (ServerRunning)
        {
            var ctx = await Listener.GetContextAsync();

            var req = ctx.Request;
            var resp = ctx.Response;

            var filePath = req?.Url?.AbsolutePath;

            if (String.IsNullOrEmpty(filePath) || filePath == "/")
            {
                filePath = "/index.html";
            }

            var fileRealPath = String.Empty;
            foreach (var dir in ServDirectories)
            {
                var p = Path.Combine(dir, filePath[1..]);
                if (File.Exists(p))
                {
                    fileRealPath = Path.GetFullPath(p);
                    break;
                }
            }


            byte[] data = [];
            if (String.IsNullOrEmpty(fileRealPath))
            {
                data = Encoding.UTF8.GetBytes(GetGoogle404Page(filePath));
                resp.ContentType = "text/html";
            }
            else
            {
                var ext = Path.GetExtension(fileRealPath).ToLower();
                data = File.ReadAllBytes(fileRealPath);
                switch (ext)
                {
                    case ".css":
                        {
                            resp.ContentType = "text/css; charset=utf-8";
                            break;
                        }
                    case ".svg":
                        {
                            resp.ContentType = "image/svg+xml";
                            break;
                        }
                    case ".png":
                        {
                            resp.ContentType = "image/png";
                            break;
                        }
                    case ".jpg":
                        {
                            resp.ContentType = "image/jpg";
                            break;
                        }
                    case ".jpeg":
                        {
                            resp.ContentType = "image/jpeg";
                            break;
                        }
                    case ".js":
                        {
                            resp.ContentType = "application/javascript; charset=utf-8";
                            break;
                        }
                    case ".json":
                        {
                            resp.ContentType = "application/json; charset=utf-8";
                            break;
                        }
                    default:
                        {
                            resp.ContentType = Utils.MimeSniffer(data);
                            break;
                        }
                }
            }
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data);
            resp.Close();
        }
    }


    // ref: https://stackoverflow.com/a/3978040
    public static int GetRandomUnusedPort()
    {
        TcpListener listener = new(IPAddress.Any, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string GetGoogle404Page(string url)
    {
        return Google404ErrorTemplate.Replace("%RequestURL%", url);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            if (ServerRunning)
            {
                Listener.Stop();
                ServerRunning = false;
            }
        }
        //dispose unmanaged resources
        Disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}