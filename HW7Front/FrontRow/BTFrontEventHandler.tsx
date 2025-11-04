import { IEventArgs } from "@docsvision/webclient/System/IEventArgs";
import { Layout } from "@docsvision/webclient/System/Layout";
import { $CardInfo } from "@docsvision/webclient/System/LayoutServices";
import { BTService, $BTService } from "./BTFrontService";
import { BTEmplInfoRequestModel,  BTPerDiemRequestModel } from "./Models/BTFrontModels";

async function fillEmployeeInfo(layout: Layout, emplId: string | undefined) {
    if (!emplId) return;

    const cardInfo = layout.getService($CardInfo);
    const svc = layout.getService<BTService>($BTService);

    const request: BTEmplInfoRequestModel = {
        cardId: cardInfo.id,
        emplId: emplId
    };

    const response = await svc.getEmployeeInfo(request);
    if (!response) return;

    const managerCtrl = layout.controls.get<any>("mngr");
    const phoneCtrl = layout.controls.get<any>("emplPhone");

    if (managerCtrl && response.mngrId)
        managerCtrl.params.value = {
            id: response.mngrId,
            name: response.mngrName
        };
  
    if (phoneCtrl)
        phoneCtrl.params.value = response.phone || "";
}

async function recalcPerDiem(layout: Layout) {
    const cardInfo = layout.getService($CardInfo);
    const svc = layout.getService<BTService>($BTService);

    const cityDDR = layout.controls.get<any>("city");  
    const fromCtrl = layout.controls.get<any>("dateStartBT");   
    const toCtrl = layout.controls.get<any>("dateFinishBT");  
    const days = layout.controls.get<any>("daysBT"); 
    const totalCtrl = layout.controls.get<any>("salaryBT");
    const cityId = (cityDDR as any).value.id ?? null;

    const request: BTPerDiemRequestModel = {
        cardId: cardInfo.id,
        cityRowId: cityId,
        dateFrom: fromCtrl?.params.value ? fromCtrl.params.value.toISOString() : null,
        dateTo: toCtrl?.params.value ? toCtrl.params.value.toISOString() : null
    };

    const response = await svc.recalculatePerDiem(request);
    if (!response) return;

    days.params.value = response.days;
    totalCtrl.params.value = response.total;
}

export async function onEmployeeChanged(sender: any, e: IEventArgs) {
    const layout: Layout = sender.layout;
    const empl = sender.params.value;
    const emplId: string | undefined = empl && (empl.id || empl.EmplId);

    await fillEmployeeInfo(layout, emplId);
}

export async function onCityChanged(sender: any, e: IEventArgs) {
    const layout: Layout = sender.layout;
    await recalcPerDiem(layout);
}

export async function onDateChanged(sender: any, e: IEventArgs) {
    const layout: Layout = sender.layout;
    await recalcPerDiem(layout);
}