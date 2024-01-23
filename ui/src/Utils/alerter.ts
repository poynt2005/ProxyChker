import { newApi, supportedApiCall } from "../Apis/ApiBase";
import { RequestMessageBox, ResponseMessageBox } from "../Apis/ApiBase";

export const customAlert = async (message: string, title: string = "Error Message"): Promise<void> => {
    const api = newApi(supportedApiCall.MB_Alert);

    try {
        const payload: RequestMessageBox = {
            Message: message,
            Title: title
        };

        api.setMessage(payload);
        const result = await api.operate() as ResponseMessageBox;
    }
    catch(e) {
        globalThis.alert(message);
    }
};

export const customInfo = async (message: string, title: string = "Info Message"): Promise<void> => {
    const api = newApi(supportedApiCall.MB_Info);

    try {
        const payload: RequestMessageBox = {
            Message: message,
            Title: title
        };

        api.setMessage(payload);
        const result = await api.operate() as ResponseMessageBox;
    }
    catch(e) {
        globalThis.alert(message);
    }
};

export const customConfirm = async (message: string, title: string = "Confirm Message"): Promise<boolean> => {
    const api = newApi(supportedApiCall.MB_Confirm);

    let isConfirm = false;
    try {
        const payload: RequestMessageBox = {
            Message: message,
            Title: title
        };

        api.setMessage(payload);
        const result = await api.operate() as ResponseMessageBox;

        isConfirm = result.IsConfirm;
    }
    catch(e) {
        isConfirm = globalThis.confirm(message);
    }

    return isConfirm;
};