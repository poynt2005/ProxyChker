using System.Runtime.InteropServices;
using SimpleHttpServer;


namespace ProxyChker.NET
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form;

#if DEBUG
            form = new();
#else
        using ServerCore core = new("127.0.0.1", ServerCore.GetRandomUnusedPort());
        core.AddServDirectory("./Statics");
        form = new(core.BindAddress);
        core.RunAsync();
#endif

            Application.Run(form);
        }

        [DllImport("user32.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SetProcessDPIAware();
    }
}

