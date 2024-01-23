import { bus } from "./bus";

declare global {
  interface Window {
    chrome: {
      webview: {
        postMessage: (message: any) => void;
        addEventListener: (listenerType: string, evt: any) => void;
      };
    };
  }
}

const hostBridge = () => {
  if (!window.chrome || !window.chrome.webview) {
    throw new Error("current context is not in a webview runtime");
  }

  const webview = window.chrome.webview;

  const webviewComponentDragEventPublisher = bus.registerPublisher(
    "webviewComponentDragEvent"
  );

  let messageQueue: any[] = [];

  webview.addEventListener("message", (evt: any) => {
    const receivedData = JSON.parse(evt.data);

    for (let i = 0; i < messageQueue.length; ++i) {
      if (messageQueue[i] && messageQueue[i].target == receivedData.Target) {
        messageQueue[i].resolver(receivedData);
        messageQueue[i] = null;
      }
    }

    messageQueue = messageQueue.filter((q) => q);
  });

  return (target: string, message: any): Promise<any> =>
    new Promise((resolve) => {
      webview.postMessage({
        Target: target,
        HtmlRequest: message,
      });

      messageQueue.push({
        target,
        resolver: (webviewResponse: any) => resolve(webviewResponse),
      });
    });
};

export const bridger = hostBridge();
