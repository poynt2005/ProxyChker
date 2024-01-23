import React, { useEffect, useState } from 'react';
import style from './App.module.css';

import NetworkBtn from './Components/NetworkBtn';
import InfoPanel, { ProxyStatus } from './Components/InfoPanel';
import { ResponseProxyChkerProxyCheckResult, ResponseProxyChkerProxyUrl, newApi, supportedApiCall } from './Apis/ApiBase';
import { customAlert } from './Utils/alerter';

const CHK_PROXY_URL_INTERVAL_SECS = 5;

const App = ():JSX.Element => {
  const [ currentProxyStatus, setCurrentProxyStatus ] = useState<ProxyStatus>(ProxyStatus.UnChecked);
  const [ currentProxyUrl, setCurrentProxyUrl ] = useState<string>("");
  const [ isProxyStatusChkingPending, setIsProxyStatusChkingPending ] = useState<boolean>(false);
  const [ currentTick, setCurrentTick ] = useState<number>(0);

  const getCurrentProxyUrl = async(): Promise<void> => {
    try{
      const api = newApi(supportedApiCall.PC_GetUrl);
      api.setMessage({});
      const { ProxyUrl } = (await api.operate()) as ResponseProxyChkerProxyUrl;
      setCurrentProxyUrl(ProxyUrl);
    }
    catch(e){
     customAlert(`Get current proxy url failed with: ${(e as Error).message}`, "Get Proxy Url Failed!!"); 
    }
  };

  const chkCurrentProxy = async(): Promise<void> => {
    try{
      const api = newApi(supportedApiCall.PC_Chk);
      api.setMessage({});
      const { IsProxyAlive } = (await api.operate()) as ResponseProxyChkerProxyCheckResult;
      
      IsProxyAlive ? setCurrentProxyStatus(ProxyStatus.Online): setCurrentProxyStatus(ProxyStatus.Down);
    }
    catch(e){
     customAlert(`Check current proxy aliving failed with: ${(e as Error).message}`, "Check Proxy Failed!!"); 
    }
  };

  useEffect(() => {
    setTimeout(() => {
      setCurrentTick(prevTick => prevTick + 1);
    }, CHK_PROXY_URL_INTERVAL_SECS * 1000);

    getCurrentProxyUrl();
  }, [currentTick]);

  useEffect(() => {
    const startChk = async(): Promise<void> => {
      await chkCurrentProxy();
      setIsProxyStatusChkingPending(false);
    }
    
    isProxyStatusChkingPending && startChk();
  }, [isProxyStatusChkingPending]);
 
  const onNetworkBtnClicked = () => {
    setIsProxyStatusChkingPending(true);
  };


  return (
    <div className={style['App']}>
      <NetworkBtn onBtnClicked={onNetworkBtnClicked} isBtnPending={isProxyStatusChkingPending}/>
      <InfoPanel proxyStatus={currentProxyStatus} proxyUrl={currentProxyUrl}/>
    </div>
  )
};

export default App;
