import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { IAviasalesControl } from "./AS/IAviasalesControl";
import { AviasalesService, $AviasalesService } from "./AS/AviasalesService";

extensionManager.registerExtension({
    name: "AviasalesExtension",
    version: "1.0",
    controls: [
        {
            controlTypeName: "AviasalesControl",
            constructor: IAviasalesControl
        }
    ],
    layoutServices: [
        Service.fromFactory(
            $AviasalesService,
            (services: $RequestManager) => new AviasalesService(services)
        )
    ]
});
