import * as EventHandlers from "./FrontRow/BTFrontEventHandler";
import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import { $BTService, BTService } from "./FrontRow/BTFrontService";
extensionManager.registerExtension({
    name: "ServerExtension",
    version: "1.0",
    globalEventHandlers: [EventHandlers],
    layoutServices: [
        Service.fromFactory($BTService, function (services) {
            return new BTService(services);
        })
    ]
});
//# sourceMappingURL=Index.js.map