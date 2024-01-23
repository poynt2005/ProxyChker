#include "ProxyChker.h"
#include "libproxychker.h"

#include <string>
#include <string.h>
#include <unordered_map>
#include <memory>

std::string strGlobalLastError("");

std::unordered_map<ProxyChkerHandle, std::unique_ptr<CProxyChker>> kContextStore;

ProxyChkerHandle ProxyChker_Create()
{
    strGlobalLastError = "";
    auto spProxyChker = std::make_unique<CProxyChker>();
    
    if (!spProxyChker->ChkIsCreated())
    {
        strGlobalLastError = CProxyChker::GetLastErrorString(spProxyChker->GetLastError());
        return 0;
    }

    auto hProxy = reinterpret_cast<ProxyChkerHandle>(spProxyChker.get());
    kContextStore.insert(std::pair<ProxyChkerHandle, std::unique_ptr<CProxyChker>>(hProxy, std::move(spProxyChker)));
    return hProxy;
}

void ProxyChker_Free(const ProxyChkerHandle hProxy)
{
    kContextStore.erase(hProxy);
}
ApiReturnCode ProxyChker_ReadProxy(const ProxyChkerHandle hProxy)
{
    auto& spProxyChker = kContextStore[hProxy];

    if (spProxyChker == nullptr)
    {
        return ApiCall_InstanceNotCreated;
    }

    if (!spProxyChker->ReadProxy())
    {
        return ApiCall_Failed;
    }

    return ApiCall_Success;
}
ApiReturnCode ProxyChker_ChkProxyAlive(const ProxyChkerHandle hProxy)
{
    auto& spProxyChker = kContextStore[hProxy];

    if (spProxyChker == nullptr)
    {
        return ApiCall_InstanceNotCreated;
    }

    if (!spProxyChker->ChkProxyAlive())
    {
        return ApiCall_Failed;
    }

    return ApiCall_Success;
}
ApiReturnCode ProxyChker_GetLastProxyIsAlive(const ProxyChkerHandle hProxy, int* pnIsAlive)
{
    auto& spProxyChker = kContextStore[hProxy];

    if (spProxyChker == nullptr)
    {
        return ApiCall_InstanceNotCreated;
    }

    *pnIsAlive = static_cast<int>(spProxyChker->GetLastProxyIsAlive());

    return ApiCall_Success;
}
ApiReturnCode ProxyChker_GetLastProxyIsEnabled(const ProxyChkerHandle hProxy, int* pnIsEnabled)
{
    auto& spProxyChker = kContextStore[hProxy];

    if (spProxyChker == nullptr)
    {
        return ApiCall_InstanceNotCreated;
    }

    *pnIsEnabled = static_cast<int>(spProxyChker->GetLastProxyIsEnabled());

    return ApiCall_Success;
}
ApiReturnCode ProxyChker_GetLastProxyUrl(const ProxyChkerHandle hProxy, char** pszProxyUrl)
{
    auto& spProxyChker = kContextStore[hProxy];

    if (spProxyChker == nullptr)
    {
        return ApiCall_InstanceNotCreated;
    }

    auto strLastProxyUrl = spProxyChker->GetLastProxyUrl();
    *pszProxyUrl = new char[strLastProxyUrl.length()+1];
    memset(*pszProxyUrl, 0 , strLastProxyUrl.length() + 1);
    memcpy(*pszProxyUrl, strLastProxyUrl.data(), strLastProxyUrl.length());

    return ApiCall_Success;
}
ApiReturnCode ProxyChker_GetLastErrorString(const ProxyChkerHandle hProxy, char** pszErrorString)
{
    if (hProxy == 0)
    {
        if (strGlobalLastError.length())
        {
            *pszErrorString = new char[strGlobalLastError.length() + 1];
            memset(*pszErrorString, 0, strGlobalLastError.length() + 1);
            memcpy(*pszErrorString, strGlobalLastError.data(), strGlobalLastError.length());
        }
        return ApiCall_Success;
    }

    auto& spProxyChker = kContextStore[hProxy];

    if (spProxyChker == nullptr)
    {
        return ApiCall_InstanceNotCreated;
    }

    auto strLastError = CProxyChker::GetLastErrorString(spProxyChker->GetLastError());

    *pszErrorString = new char[strLastError.length() + 1];
    memset(*pszErrorString, 0, strLastError.length() + 1);
    memcpy(*pszErrorString, strLastError.data(), strLastError.length());

    return ApiCall_Success;
}
void ProxyChker_FreeBuffer(void** ppBuffer)
{
    if (ppBuffer == nullptr || *ppBuffer == nullptr)
    {
        return;
    }

    delete[] * ppBuffer;
    *ppBuffer = nullptr;
}