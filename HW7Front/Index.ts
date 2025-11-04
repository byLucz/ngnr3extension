import * as EventHandlers from "./FrontRow/BTFrontEventHandler";
import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { $BTService, BTService } from "./FrontRow/BTFrontService";

extensionManager.registerExtension({
    name: "ServerExtension",
    version: "1.0",
    globalEventHandlers: [EventHandlers],
    layoutServices: [
        Service.fromFactory($BTService, (services: $RequestManager) =>
            new BTService(services)
        )
    ]
});
