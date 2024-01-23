using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace ProxyChker
{
    enum ApiReturnCodeDef
    {
        ApiCall_InstanceNotCreated = 0,
        ApiCall_Success,
        ApiCall_Failed,
    }

    public class ProxyChkerCore : IDisposable
    {
        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern UInt64 ProxyChker_Create();

        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void ProxyChker_Free(UInt64 hProxy);

        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void ProxyChker_FreeBuffer(ref IntPtr ppBuffer);

        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ProxyChker_ReadProxy(UInt64 hProxy);

        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ProxyChker_ChkProxyAlive(UInt64 hProxy);

        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ProxyChker_GetLastProxyIsAlive(UInt64 hProxy, ref int pnIsAlive);

        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ProxyChker_GetLastProxyIsEnabled(UInt64 hProxy, ref int pnIsEnabled);

        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ProxyChker_GetLastProxyUrl(UInt64 hProxy, ref IntPtr pszProxyUrl);

        [DllImport("libproxychker", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ProxyChker_GetLastErrorString(UInt64 hProxy, ref IntPtr pszErrorString);

        public bool LastProxyIsAlive
        {
            get
            {
                int isAlive = 0;
                InterpretException(ProxyChkerHandle, (ApiReturnCodeDef)ProxyChker_GetLastProxyIsAlive(ProxyChkerHandle, ref isAlive));
                return Convert.ToBoolean(isAlive);
            }
        }

        public bool LastProxyIsEnabled
        {
            get
            {
                int isEnabled = 0;
                InterpretException(ProxyChkerHandle, (ApiReturnCodeDef)ProxyChker_GetLastProxyIsEnabled(ProxyChkerHandle, ref isEnabled));
                return Convert.ToBoolean(isEnabled);
            }
        }

        public string LastProxyUrl
        {
            get
            {
                var proxyUrlStrPtr = IntPtr.Zero;
                InterpretException(ProxyChkerHandle, (ApiReturnCodeDef)ProxyChker_GetLastProxyUrl(ProxyChkerHandle, ref proxyUrlStrPtr));

                if (proxyUrlStrPtr == IntPtr.Zero)
                {
                    throw new Exception("cannot get last proxy url result, context returned a null pointer");
                }

                var proxyUrlStr = Marshal.PtrToStringAnsi(proxyUrlStrPtr);
                ProxyChker_FreeBuffer(ref proxyUrlStrPtr);

                if (String.IsNullOrEmpty(proxyUrlStr))
                {
                    throw new Exception("cannot get last proxy url result, cannot marshal from pointer to string");
                }
                return proxyUrlStr;
            }
        }

        private bool Disposed { get; set; } = false;
        private UInt64 ProxyChkerHandle { get; set; } = 0;

        public ProxyChkerCore()
        {
            ProxyChkerHandle = ProxyChker_Create();

            if (ProxyChkerHandle == 0)
            {
                var createErrReasonPtr = IntPtr.Zero;

                _ = ProxyChker_GetLastErrorString(0, ref createErrReasonPtr);

                var createErrReason = "cannot create proxychker instance, unknown error";
                if (createErrReasonPtr == IntPtr.Zero)
                {
                    throw new Exception(createErrReason);
                }

                createErrReason = Marshal.PtrToStringAnsi(createErrReasonPtr);
                ProxyChker_FreeBuffer(ref createErrReasonPtr);

                if (String.IsNullOrEmpty(createErrReason))
                {
                    throw new Exception("cannot create proxychker instance with an unknown error, cannot marshal from pointer to string");
                }

                throw new Exception(createErrReason);
            }
        }

        public void ReadProxy()
        {
            InterpretException(ProxyChkerHandle, (ApiReturnCodeDef)ProxyChker_ReadProxy(ProxyChkerHandle));
        }

        public void ChkProxyAlive()
        {
            InterpretException(ProxyChkerHandle, (ApiReturnCodeDef)ProxyChker_ChkProxyAlive(ProxyChkerHandle));
        }

        private static void InterpretException(UInt64 hProxyHandle, ApiReturnCodeDef ret)
        {
            if (ret == ApiReturnCodeDef.ApiCall_InstanceNotCreated)
            {
                throw new Exception($"proxyychker instance is not created in go context");
            }

            if (ret == ApiReturnCodeDef.ApiCall_Failed)
            {
                var errReasonStrPtr = IntPtr.Zero;
                var getErrRet = (ApiReturnCodeDef)ProxyChker_GetLastErrorString(hProxyHandle, ref errReasonStrPtr);

                if (getErrRet == ApiReturnCodeDef.ApiCall_InstanceNotCreated)
                {
                    throw new Exception($"cannot get last error, instance is not created");
                }

                if (errReasonStrPtr == IntPtr.Zero)
                {
                    throw new Exception("api call encountered a unknown error");
                }
                else
                {
                    var errReason = Marshal.PtrToStringAnsi(errReasonStrPtr);
                    ProxyChker_FreeBuffer(ref errReasonStrPtr);

                    if (String.IsNullOrEmpty(errReason))
                    {
                        throw new Exception("unknown error, cannot marshal from pointer to string");
                    }

                    throw new Exception(errReason);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // Dispose unmanaged resources
                }

                if (ProxyChkerHandle != 0)
                {
                    ProxyChker_Free(ProxyChkerHandle);
                    ProxyChkerHandle = 0;
                }

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}