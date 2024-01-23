using System.Runtime.InteropServices;

namespace SimpleHttpServer.Lib
{
    enum HRESULT : UInt64
    {
        S_OK = 0x00000000,
        E_ABORT = 0x80004004,
        E_ACCESSDENIED = 0x80070005,
        E_FAIL = 0x80004005,
        E_HANDLE = 0x80070006,
        E_INVALIDARG = 0x80070057,
        E_NOINTERFACE = 0x80004002,
        E_NOTIMPL = 0x80004001,
        E_OUTOFMEMORY = 0x8007000E,
        E_POINTER = 0x80004003,
        E_UNEXPECTED = 0x8000FFFF,
    }

    static class Utils
    {
        [DllImport("Urlmon.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt64 FindMimeFromData(IntPtr pBC, string? pwzUrl, byte[]? pBuffer, UInt64 cbSize, string? pwzMimeProposed, UInt64 dwMimeFlags, ref string ppwzMimeOut, UInt64 dwReserved);

        public static string MimeSniffer(byte[] fileContent)
        {
            var mimeOut = String.Empty;
            var hResult = (HRESULT)FindMimeFromData(IntPtr.Zero, null, fileContent, (UInt64)fileContent.LongLength, null, 0, ref mimeOut, 0);

            if (hResult == HRESULT.S_OK)
            {
                return mimeOut;
            }

            return "application/octet-stream";
        }

    }
}