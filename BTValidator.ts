import { Layout } from "@docsvision/webclient/System/Layout";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";

export interface CheckResult {
    ok: boolean;
    msg?: string;
}

export function getTextValue(layout: Layout, controlName: string): string | null {
    const ctrl = layout.controls.get<TextBox>(controlName);
    if (!ctrl) return null;

    return (ctrl as any).value ?? null;
}

export function getDateValue(layout: Layout, controlName: string): Date | null {
    const ctrl = layout.controls.get<DateTimePicker>(controlName);
    if (!ctrl) return null;

    const raw = (ctrl as any).value as any;
    if (!raw) return null;

    if (raw instanceof Date) {
        return raw;
    }

    const parsed = new Date(raw);
    return isNaN(parsed.getTime()) ? null : parsed;
}

export function validateReasonBT(layout: Layout): CheckResult {
    const reason = getTextValue(layout, "reasonBT");

    if (!reason || reason.trim() === "") {
        return {
            ok: false,
            msg: "Основание для поездки не заполнено, cохранение отменено!"
        };
    }

    return { ok: true };
}

export function validateDates(layout: Layout): CheckResult {
    const fromDate = getDateValue(layout, "dateStartBT");
    const toDate = getDateValue(layout, "dateFinishBT");

    if (!fromDate || !toDate) {
        return { ok: true };
    }

    if (toDate < fromDate) {
        return {
            ok: false,
            msg: "'Дата по' не может быть раньше 'даты с'!"
        };
    }

    return { ok: true };
}

export function collectCardInfo(layout: Layout) {
    const title = getTextValue(layout, "cardName") ?? "(без названия)";
    const created = getDateValue(layout, "cardDate")
    const reason = getTextValue(layout, "reasonBT") ?? "(без основания)";
    const fromDate = getDateValue(layout, "dateStartBT");
    const toDate = getDateValue(layout, "dateFinishBT");
    const cityDDR = layout.controls.get("city");
    const city = (cityDDR as any).value.name ?? null;

    function parseDate(d: Date | null | undefined): string {
        if (!d) return "(не указана)";

        if (!(d instanceof Date)) {
            const tryDate = new Date(d as any);
            if (!isNaN(tryDate.getTime())) return parseDate(tryDate);
            return String(d);
        }

        const dd = String(d.getDate()).padStart(2, "0");
        const mm = String(d.getMonth() + 1).padStart(2, "0");
        const yyyy = d.getFullYear();
        const hh = String(d.getHours()).padStart(2, "0");
        const min = String(d.getMinutes()).padStart(2, "0");
        return `${dd}.${mm}.${yyyy} ${hh}:${min}`;
    }

    return {
        title,
        created: parseDate(created),
        fromDate: parseDate(fromDate),
        toDate: parseDate(toDate),
        city,
        reason
    };
}
