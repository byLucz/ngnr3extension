import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import * as bteh from "./EventHandlers/BTEventHandler";

extensionManager.registerExtension({
    name: "NGNR3Extension",
    version: "1.0.0",
    globalEventHandlers: [bteh],
    layoutServices: []
});