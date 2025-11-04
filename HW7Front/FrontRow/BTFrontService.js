import { urlStore } from "@docsvision/webclient/System/UrlStore";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";
var BTService = /** @class */ (function () {
    function BTService(services) {
        this.services = services;
    }
    BTService.prototype.getEmployeeInfo = function (request) {
        var url = urlStore.urlResolver.resolveApiUrl("GetEmployeeInfo", "BT");
        return this.services.requestManager.post(url, JSON.stringify(request));
    };
    BTService.prototype.recalculatePerDiem = function (request) {
        var url = urlStore.urlResolver.resolveApiUrl("RecalculatePerDiem", "BT");
        return this.services.requestManager.post(url, JSON.stringify(request));
    };
    return BTService;
}());
export { BTService };
export var $BTService = serviceName(function (s) { return s.btService; });
//# sourceMappingURL=BTFrontService.js.map