import * as React from "react";
import { BaseControlParams, BaseControlState } from "@docsvision/webclient/System/BaseControl";
import { BaseControlImpl } from "@docsvision/webclient/System/BaseControlImpl";
import { Layout } from "@docsvision/webclient/System/Layout";
import { $CardInfo } from "@docsvision/webclient/System/LayoutServices";
import { $AviasalesService } from "./AviasalesService";
import { AvRequest, TicketParams } from "./Models/AviaModels";

export class AviasalesControlParams extends BaseControlParams {
    cityControlName: string = "city";
    dateFromControlName: string = "dateStartBT";
    dateToControlName: string = "dateFinishBT";
    panelOpened: boolean;
}

export interface AviasalesControlState extends AviasalesControlParams, BaseControlState {
    isLoading: boolean;
    tickets: TicketParams[];
    selectedIndex: number | null;
    error?: string;
}

export class AviasalesControlImpl extends BaseControlImpl<AviasalesControlParams, AviasalesControlState> {
    constructor(props: AviasalesControlParams, state: AviasalesControlState) {
        super(props, state);
        this.state.isLoading = false;
        this.state.tickets = [];
        this.state.selectedIndex = null;
        this.state.error = undefined;
        this.state.panelOpened = false;
    }

    private get layout(): Layout {
        return (this.state.wrapper as any).layout as Layout;
    }

    private get aviaService() {
        return this.layout.getService($AviasalesService);
    }

    private onRequestTickets = async () => {
        const { cityControlName, dateFromControlName, dateToControlName } = this.state;
        const layout = this.layout;
        const cardInfo = layout.getService($CardInfo);
        const cityCtrl = layout.controls.get<any>(cityControlName);
        const fromCtrl = layout.controls.get<any>(dateFromControlName);
        const toCtrl = layout.controls.get<any>(dateToControlName);
        const cityVal = cityCtrl.params.value as any;
        const cityRowId = cityVal.id;
        const dateFrom = fromCtrl.params.value as Date;
        const dateTo = toCtrl.params.value as Date;
        const today = new Date();

        if (!cityRowId || !dateFrom || !dateTo) {
            this.setState({ error: "Укажите город и даты командировки", panelOpened: true, isLoading: false });
            return;
        }

        if (dateTo < today || dateFrom < today) {
            this.setState({ error: "Нельзя выбирать прошедшие даты командировки", panelOpened: true, isLoading: false });
            return;
        }

        if (cityRowId == "81f052de-b488-4ef0-8733-227c6d4a765c") {
            this.setState({ error: "Нельзя выбирать тот же город, что и город офиса", panelOpened: true, isLoading: false });
            return;
        }

        const request: AvRequest = {
            cardId: cardInfo.id,
            cityRowId,
            dateFrom: dateFrom.toISOString(),
            dateTo: dateTo.toISOString()
        };

        this.setState({ isLoading: true, error: undefined, panelOpened: true });

        try {
            const tickets = await this.aviaService.getTickets(request);

            this.setState({ tickets, selectedIndex: tickets.length ? 0 : null, isLoading: false });
        } catch {
            this.setState({ error: "Ошибка получения данных от сервера", isLoading: false });
        }
    };

    private onTicketChanged = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const index = parseInt(e.target.value, 10);
        this.setState({ selectedIndex: index });
    };

    protected renderControl(): JSX.Element {
        if (!this.state.visibility) return null as any;

        const { isLoading, tickets, selectedIndex, error, panelOpened } = this.state;
        const selectedTicket =
            selectedIndex != null ? tickets[selectedIndex] : null;

        return (
            <div className="dv-aviasales-control">
                {}
                <button type="button" disabled={isLoading} onClick={this.onRequestTickets} className="dv-as-button">{
                    isLoading ? "Запрос выполняется..." : "Запросить стоимость билетов"}
                </button>

                {}
                {
                panelOpened && (
                <div className="dv-as-panel">
                    <div className="dv-as-panel-row">
                        <select className="dv-as-select" disabled={!tickets.length} value={selectedIndex ?? 0} onChange={this.onTicketChanged}>{
                            tickets.length === 0 ? (<option>Варианты билетов</option>) : (tickets.map((t, i) =>
                                (   <option key={i} value={i}>
                                    {t.airline} ({t.flightNum}) - {t.price} руб.
                                    </option>
                                ))
                            )}
                        </select>
                        <div className="dv-as-price">
                            {selectedTicket ? `Стоимость билетов: ${selectedTicket.price} руб.` : "Стоимость билетов:"}
                        </div>
                    </div>
                    {error && <div className="dv-as-error">{error}</div>}
                    </div>)
                }
            </div>
        );
    }
}
