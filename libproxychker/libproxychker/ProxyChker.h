#pragma once

#include <Windows.h>
#include <atlbase.h>
#include <exdisp.h>
#include <httprequest.h>
#include <string>
#include <optional>

enum class EProxyChkerError
{
    NoError = 0,
    Com_CoInitializeExFailed,
    Com_GetWScriptClsIdFailed,
    Com_GetWinHTTPClsIdFailed,
    Com_GetWsDispatcherFailed,
    Com_GetHttpRequestFailed,
    Com_NotCreated,
    Com_DispatchInvokeFailed,
    Com_DispatchGetIdsOfNamesFailed,
    Com_SetProxyFailed,
    Com_OpenHttpRequestFailed,
    Com_GetHttpResponseStatusFailed,
    Proxy_NotRead,
};

using CLastProxyChkResult = struct cLastProxyChkResult
{
    bool m_bIsEnabled;
    std::wstring m_wstrProxyUrl;
};

class CProxyChker
{
public:
    CProxyChker();
    ~CProxyChker();

    bool ReadProxy();
    bool ChkProxyAlive();
    bool ChkIsCreated() const { return m_bIsComCreated; }

    bool GetLastProxyIsAlive() const { return m_bLastProxyChkIsAlive; }
    bool GetLastProxyIsEnabled() const { return m_kLastProxyChkResult.value().m_bIsEnabled; }
    std::string GetLastProxyUrl() const { return WstrToStr(m_kLastProxyChkResult.value().m_wstrProxyUrl); }
    EProxyChkerError GetLastError() const { return m_eLastError; }

    static std::string GetLastErrorString(EProxyChkerError eError);
private:
    std::optional<CLastProxyChkResult> m_kLastProxyChkResult = std::nullopt;
    bool m_bLastProxyChkIsAlive = false;

    CComPtr<IDispatch> m_spShellDispatcher = nullptr;
    CComPtr<IWinHttpRequest> m_spHttpRequest = nullptr;
    

    bool m_bIsComCreated = false;

    EProxyChkerError m_eLastError = EProxyChkerError::NoError;

    static std::string WstrToStr(const std::wstring& wstrInput);
};