#ifndef __LIB_PROXY_CHKER_H__
#define __LIB_PROXY_CHKER_H__

#include <stdint.h>
#include <stddef.h>

#define ProxyChkerHandle uint64_t

typedef enum __apiReturnCode
{
    ApiCall_InstanceNotCreated = 0,
    ApiCall_Success,
    ApiCall_Failed,
} ApiReturnCode;

#ifdef __cplusplus
extern "C"
{
#endif

    __declspec(dllexport) ProxyChkerHandle ProxyChker_Create();
    __declspec(dllexport) void ProxyChker_Free(const ProxyChkerHandle hProxy);
    __declspec(dllexport) ApiReturnCode ProxyChker_ReadProxy(const ProxyChkerHandle hProxy);
    __declspec(dllexport) ApiReturnCode ProxyChker_ChkProxyAlive(const ProxyChkerHandle hProxy);
    __declspec(dllexport) ApiReturnCode ProxyChker_GetLastProxyIsAlive(const ProxyChkerHandle hProxy, int* pnIsAlive);
    __declspec(dllexport) ApiReturnCode ProxyChker_GetLastProxyIsEnabled(const ProxyChkerHandle hProxy, int* pnIsEnabled);
    __declspec(dllexport) ApiReturnCode ProxyChker_GetLastProxyUrl(const ProxyChkerHandle hProxy, char** pszProxyUrl);
    __declspec(dllexport) ApiReturnCode ProxyChker_GetLastErrorString(const ProxyChkerHandle hProxy, char** pszErrorString);
    __declspec(dllexport) void ProxyChker_FreeBuffer(void **ppBuffer);


#ifdef __cplusplus
}
#endif

#endif
