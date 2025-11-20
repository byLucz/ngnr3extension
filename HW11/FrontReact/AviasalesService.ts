import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { urlStore } from "@docsvision/webclient/System/UrlStore";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";
import { AvRequest, TicketParams } from "./Models/AviaModels";

export class AviasalesService {
    constructor(private services: $RequestManager) {}

    getTickets(model: AvRequest): Promise<TicketParams[]> {
        const url = urlStore.urlResolver.resolveApiUrl("GetTickets", "ASE");
        return this.services.requestManager.post<TicketParams[]>(
            url,
            JSON.stringify(model)
        );
    }
}

export type $AviasalesService = { aviasalesService: AviasalesService };
export const $AviasalesService = serviceName(
    (s: $AviasalesService) => s.aviasalesService
);
