import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { urlStore } from "@docsvision/webclient/System/UrlStore";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";
import { BTEmplInfoRequestModel,BTEmployeeInfoModel, BTPerDiemRequestModel, BTPerDiemModel } from "./Models/BTFrontModels";

export class BTService {
    constructor(private services: $RequestManager) { }

    getEmployeeInfo(request: BTEmplInfoRequestModel): Promise<BTEmployeeInfoModel> {
        const url = urlStore.urlResolver.resolveApiUrl("GetEmployeeInfo", "BT");
        return this.services.requestManager.post<BTEmployeeInfoModel>(url, JSON.stringify(request));
    }

    recalculatePerDiem(request: BTPerDiemRequestModel): Promise<BTPerDiemModel> {
        const url = urlStore.urlResolver.resolveApiUrl("RecalculatePerDiem", "BT");
        return this.services.requestManager.post<BTPerDiemModel>(url, JSON.stringify(request));
    }
}

export type $BTService = { btService: BTService };
export const $BTService = serviceName((s: $BTService) => s.btService);
