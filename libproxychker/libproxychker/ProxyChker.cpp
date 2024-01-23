#include <Windows.h>

#include <memory>
#include <string>
#include <string.h>

#include "ProxyChker.h"

CProxyChker::CProxyChker()
{
    HRESULT hr;
    if (SUCCEEDED((hr = CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED))))
    {
        CLSID kWsClsId;
        if (SUCCEEDED((hr = CLSIDFromProgID(L"WScript.Shell", &kWsClsId))))
        {
            CLSID kWinHttpClsId;
            if (SUCCEEDED((hr = CLSIDFromProgID(L"WinHTTP.WinHTTPRequest.5.1", &kWinHttpClsId))))
            {
                CComPtr<IDispatch> spDispatch = nullptr;
                if (SUCCEEDED((hr = spDispatch.CoCreateInstance(kWsClsId, nullptr, CLSCTX_ALL))))
                {
                    m_spShellDispatcher = std::move(spDispatch);

                    CComPtr<IWinHttpRequest> spHttpRequest = nullptr;
                    if (SUCCEEDED((hr = spHttpRequest.CoCreateInstance(kWinHttpClsId, nullptr, CLSCTX_ALL))))
                    {
                        m_spHttpRequest = std::move(spHttpRequest);
                        m_bIsComCreated = true;
                    }
                    else
                    {
                        m_eLastError = EProxyChkerError::Com_GetHttpRequestFailed;
                    }
                }
                else
                {
                    m_eLastError = EProxyChkerError::Com_GetWsDispatcherFailed;
                }
            }
            else
            {
                m_eLastError = EProxyChkerError::Com_GetWinHTTPClsIdFailed;
            }
        }
        else
        {
            m_eLastError = EProxyChkerError::Com_GetWScriptClsIdFailed;
        }

    }
    else
    {
        m_eLastError = EProxyChkerError::Com_CoInitializeExFailed;
    }
}

CProxyChker::~CProxyChker()
{
    if (m_bIsComCreated)
    {
        CoUninitialize();
    }
}

bool CProxyChker::ReadProxy()
{
    if (!m_bIsComCreated)
    {
        m_eLastError = EProxyChkerError::Com_NotCreated;
        return false;
    }

    m_eLastError = EProxyChkerError::NoError;
    m_kLastProxyChkResult = std::nullopt;

    HRESULT hr;
    const auto kFnRegRead = [&](const std::wstring& wstrRegPath, CComVariant& vtResult) -> EProxyChkerError
        {
            CComBSTR bstrFnName(L"RegRead");
            DISPID kDispid;
            if (SUCCEEDED((hr = m_spShellDispatcher->GetIDsOfNames(IID_NULL, &bstrFnName, 1, LOCALE_SYSTEM_DEFAULT, &kDispid))))
            {
                CComVariant vtParams[1] = { CComVariant(wstrRegPath.c_str()) };

                DISPPARAMS kParams
                {
                    vtParams,
                    nullptr,
                    1,
                    0
                };

                if (SUCCEEDED((hr = m_spShellDispatcher->Invoke(kDispid, IID_NULL, LOCALE_SYSTEM_DEFAULT, DISPATCH_METHOD, &kParams, &vtResult, nullptr, nullptr))))
                {
                    return EProxyChkerError::NoError;
                }
                else
                {
                    return EProxyChkerError::Com_DispatchInvokeFailed;
                }
            }
            else
            {
                return EProxyChkerError::Com_DispatchGetIdsOfNamesFailed;
            }
        };
    
    CComVariant vtRegUrl;
    if ((m_eLastError = kFnRegRead(L"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\ProxyServer", vtRegUrl)) == EProxyChkerError::NoError)
    {
        CComVariant vtRegIsEnable;
        if ((m_eLastError = kFnRegRead(L"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\ProxyEnable", vtRegIsEnable)) == EProxyChkerError::NoError)
        {
            CLastProxyChkResult kResult
            {
                !!vtRegIsEnable.lVal,
                std::wstring(vtRegUrl.bstrVal)
            };

            m_kLastProxyChkResult = std::move(kResult);
            return true;
        }
    }
    return false;
}

bool CProxyChker::ChkProxyAlive()
{
    if (!m_bIsComCreated)
    {
        m_eLastError = EProxyChkerError::Com_NotCreated;
        return false;
    }

    if (!m_kLastProxyChkResult.has_value())
    {
        m_eLastError = EProxyChkerError::Proxy_NotRead;
        return false;
    }

    m_eLastError = EProxyChkerError::NoError;
    m_bLastProxyChkIsAlive = false;

    auto& kProxy = m_kLastProxyChkResult.value();

    CComVariant vtProxyUrl(kProxy.m_wstrProxyUrl.c_str());
    CComVariant vtByPassUrl;

    HRESULT hr;
    if (SUCCEEDED((hr = m_spHttpRequest->SetProxy(HTTPREQUEST_PROXYSETTING_PROXY, vtProxyUrl, vtByPassUrl))))
    {
        CComBSTR bstrHttpMethod(L"GET");
        CComBSTR bstrHttpTargetUrl(L"http://www.msftconnecttest.com/connecttest.txt");
        CComVariant vtIsAsync(false);

        if (SUCCEEDED((hr = m_spHttpRequest->Open(bstrHttpMethod, bstrHttpTargetUrl, vtIsAsync))))
        {
            CComVariant vtRequestBody;
            if (SUCCEEDED((hr = m_spHttpRequest->Send(vtRequestBody))))
            {
                long lStatus = -1;
                
                if (SUCCEEDED((hr = m_spHttpRequest->get_Status(&lStatus))))
                {
                    m_bLastProxyChkIsAlive = (lStatus >= 200L && lStatus < 400L);
                    return true;
                }
                else
                {
                    m_eLastError = EProxyChkerError::Com_GetHttpResponseStatusFailed;
                }
            }
            else
            {
                // 送出失敗，代表沒有連線
                m_bLastProxyChkIsAlive = false;
                return true;
            }
        }
        else
        {
            m_eLastError = EProxyChkerError::Com_OpenHttpRequestFailed;
        }
    }
    else
    {
        m_eLastError = EProxyChkerError::Com_SetProxyFailed;
    }

    return false;
}

std::string CProxyChker::WstrToStr(const std::wstring& wstrInput)
{
    std::string strReturned;
    SIZE_T ulCount = WideCharToMultiByte(CP_UTF8, 0, wstrInput.c_str(), -1, nullptr, 0, nullptr, nullptr);

    if (ulCount)
    {
        auto spBuffer = std::make_unique<char[]>(ulCount + 1);
        memset(spBuffer.get(), 0, ulCount + 1);
        
        auto ulResult = WideCharToMultiByte(CP_UTF8, 0, wstrInput.c_str(), -1, spBuffer.get(), ulCount, nullptr, nullptr);
    
        if (ulResult)
        {
            strReturned = std::string(spBuffer.get());
        }
    }
    return strReturned;
}

std::string CProxyChker::GetLastErrorString(EProxyChkerError eError)
{
    switch (eError)
    {
    case EProxyChkerError::NoError:
    {
        return "no error";
    }
    case EProxyChkerError::Com_CoInitializeExFailed:
    {
        return "cannot initialize com library";
    }
    case EProxyChkerError::Com_GetWScriptClsIdFailed: 
    {
        return "cannot get wscript.shell clsid";
    }
    case EProxyChkerError::Com_GetWinHTTPClsIdFailed:
    {
        return "cannot get winhttp clsid";
    }
    case EProxyChkerError::Com_GetWsDispatcherFailed:
    {
        return "cannot get wscript dispatcher com interface";
    }
    case EProxyChkerError::Com_GetHttpRequestFailed:
    {
        return "cannot get httprequest com interface";
    }
    case EProxyChkerError::Com_NotCreated:
    {
        return "com interface is not created";
    }
    case EProxyChkerError::Com_DispatchInvokeFailed:
    {
        return "wscript dispatcher method invoke called failed";
    }
    case EProxyChkerError::Com_DispatchGetIdsOfNamesFailed:
    {
        return "wscript dispatcher method getidsofnames called failed";
    }
    case EProxyChkerError::Com_SetProxyFailed:
    {
        return "http request set proxy failed";
    }
    case EProxyChkerError::Com_OpenHttpRequestFailed:
    {
        return "http request open failed";
    }
    case EProxyChkerError::Com_GetHttpResponseStatusFailed:
    {
        return "http response status get failed";
    }
    case EProxyChkerError::Proxy_NotRead:
    {
        return "proxy is not read, you must read proxy first";
    }
    }

    return "";
}