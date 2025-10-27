import { Layout } from "@docsvision/webclient/System/Layout";
import { IEventArgs } from "@docsvision/webclient/System/IEventArgs";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { CancelableEventArgs } from "@docsvision/webclient/System/CancelableEventArgs";
import { ICardSavingEventArgs } from "@docsvision/webclient/System/ICardSavingEventArgs";
import { validateReasonBT, validateDates, collectCardInfo } from "../Logic/BTValidator";

export async function checkCardBeforeSave(layout: Layout, args: CancelableEventArgs<ICardSavingEventArgs>): Promise<void> {
    if (!layout) return;

    args.wait();
    const checkReason = validateReasonBT(layout);

    if (!checkReason.ok) {
        await MessageBox.ShowWarning(checkReason.msg);
        args.cancel();
        return;
    }

    args.accept();
}

export async function checkDateChanged(sender: any, e: IEventArgs): Promise<void> {
    const layout: Layout = sender.layout ?? sender;

    const dateCheck = validateDates(layout);
    if (!dateCheck.ok && dateCheck.msg) {
        await MessageBox.ShowWarning(dateCheck.msg);
    }
}

export async function clickToCollectCardInfo(sender: any, e: IEventArgs): Promise<void> {
    const layout: Layout = sender.layout ?? sender;

    const info = collectCardInfo(layout);

    const text =
        "Карточка: " + info.title + "\n" +
        "Создана: " + info.created + "\n" +
        "Дата с: " + info.fromDate + "\n" +
        "Дата по: " + info.toDate + "\n" +
        "Город: " + info.city + "\n" +
        "Основание: " + info.reason;

    await MessageBox.ShowInfo(text);
}