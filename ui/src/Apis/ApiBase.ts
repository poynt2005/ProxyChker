import { bridger } from "../Utils/bridge";

export enum supportedApiCall {
  // MessageBox Rel
  MB_Alert,
  MB_Confirm,
  MB_Info,

  // proxychker rel
  PC_GetUrl,
  PC_Chk,
}

export interface RequestBase {}

export interface RequestMessageBox extends RequestBase {
  Message: string;
  Title: string;
}

export interface RequestProxyChkerGetProxyUrl extends RequestBase {}
export interface RequestProxyChkerCheckProxyAlive extends RequestBase {}

interface ResponseBase {
  Target?: string;
  IsSuccess?: boolean;
  ErrorReason?: string;
}

export interface ResponseMessageBox extends ResponseBase {
  IsConfirm: boolean;
}

export interface ResponseProxyChkerProxyUrl extends ResponseBase {
  ProxyUrl: string;
}

export interface ResponseProxyChkerProxyCheckResult extends ResponseBase {
  IsProxyAlive: boolean;
}

class Api {
  private target: string;
  private verb: string;
  private message: any = {};

  constructor(target: string, verb: string) {
    this.target = target;
    this.verb = verb;
  }

  setMessage<
    T extends
      | RequestBase
      | RequestMessageBox
      | RequestProxyChkerGetProxyUrl
      | RequestProxyChkerCheckProxyAlive
  >(message: T): void {
    this.message = message;
  }

  async operate(): Promise<ResponseBase> {
    const rawResult = (await bridger(
      `${this.target}_${this.verb}`,
      this.message
    )) as ResponseBase;

    if (!rawResult.IsSuccess) {
      return Promise.reject(new Error(rawResult.ErrorReason));
    }

    const result = JSON.parse(JSON.stringify(rawResult));

    delete result.Target;
    delete result.IsSuccess;
    delete result.ErrorReason;

    return result;
  }

  static createApi(eSupportedApiCall: supportedApiCall): Api {
    switch (eSupportedApiCall) {
      case supportedApiCall.MB_Alert: {
        return new Api("MessageBox", "Alert");
      }
      case supportedApiCall.MB_Confirm: {
        return new Api("MessageBox", "Confirm");
      }
      case supportedApiCall.MB_Info: {
        return new Api("MessageBox", "Info");
      }

      case supportedApiCall.PC_Chk: {
        return new Api("ProxyChker", "Chk");
      }
      case supportedApiCall.PC_GetUrl: {
        return new Api("ProxyChker", "GetUrl");
      }
      default: {
        throw new Error(`target enum ${eSupportedApiCall} is not supported`);
      }
    }
  }
}

export const newApi = (eSupportedApiCall: supportedApiCall) =>
  Api.createApi(eSupportedApiCall);
