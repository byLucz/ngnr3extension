define(['@docsvision/webclient/System/ExtensionManager', 'tslib', '@docsvision/webclient/Helpers/MessageBox/MessageBox'], (function (ExtensionManager, tslib, MessageBox) { 'use strict';

    function getTextValue(layout, controlName) {
        var _a;
        var ctrl = layout.controls.get(controlName);
        if (!ctrl)
            return null;
        return (_a = ctrl.value) !== null && _a !== void 0 ? _a : null;
    }
    function getDateValue(layout, controlName) {
        var ctrl = layout.controls.get(controlName);
        if (!ctrl)
            return null;
        var raw = ctrl.value;
        if (!raw)
            return null;
        if (raw instanceof Date) {
            return raw;
        }
        var parsed = new Date(raw);
        return isNaN(parsed.getTime()) ? null : parsed;
    }
    function validateReasonBT(layout) {
        var reason = getTextValue(layout, "reasonBT");
        if (!reason || reason.trim() === "") {
            return {
                ok: false,
                msg: "Основание для поездки не заполнено, cохранение отменено!"
            };
        }
        return { ok: true };
    }
    function validateDates(layout) {
        var fromDate = getDateValue(layout, "dateStartBT");
        var toDate = getDateValue(layout, "dateFinishBT");
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
    function collectCardInfo(layout) {
        var _a, _b;
        var title = (_a = getTextValue(layout, "cardName")) !== null && _a !== void 0 ? _a : "(без названия)";
        var created = getDateValue(layout, "cardDate");
        var reason = (_b = getTextValue(layout, "reasonBT")) !== null && _b !== void 0 ? _b : "(без основания)";
        var fromDate = getDateValue(layout, "dateStartBT");
        var toDate = getDateValue(layout, "dateFinishBT");
        function parseDate(d) {
            if (!d)
                return "(не указана)";
            if (!(d instanceof Date)) {
                var tryDate = new Date(d);
                if (!isNaN(tryDate.getTime()))
                    return parseDate(tryDate);
                return String(d);
            }
            var dd = String(d.getDate()).padStart(2, "0");
            var mm = String(d.getMonth() + 1).padStart(2, "0");
            var yyyy = d.getFullYear();
            var hh = String(d.getHours()).padStart(2, "0");
            var min = String(d.getMinutes()).padStart(2, "0");
            return dd + "." + mm + "." + yyyy + " " + hh + ":" + min;
        }
        return {
            title: title,
            created: parseDate(created),
            fromDate: parseDate(fromDate),
            toDate: parseDate(toDate),
            reason: reason
        };
    }

    function checkCardBeforeSave(layout, args) {
        return tslib.__awaiter(this, void 0, void 0, function () {
            var checkReason;
            return tslib.__generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!layout)
                            return [2 /*return*/];
                        args.wait();
                        checkReason = validateReasonBT(layout);
                        if (!!checkReason.ok) return [3 /*break*/, 2];
                        return [4 /*yield*/, MessageBox.MessageBox.ShowWarning(checkReason.msg)];
                    case 1:
                        _a.sent();
                        args.cancel();
                        return [2 /*return*/];
                    case 2:
                        args.accept();
                        return [2 /*return*/];
                }
            });
        });
    }
    function checkDateChanged(sender, e) {
        var _a;
        return tslib.__awaiter(this, void 0, void 0, function () {
            var layout, dateCheck;
            return tslib.__generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        layout = (_a = sender.layout) !== null && _a !== void 0 ? _a : sender;
                        dateCheck = validateDates(layout);
                        if (!(!dateCheck.ok && dateCheck.msg)) return [3 /*break*/, 2];
                        return [4 /*yield*/, MessageBox.MessageBox.ShowWarning(dateCheck.msg)];
                    case 1:
                        _b.sent();
                        _b.label = 2;
                    case 2: return [2 /*return*/];
                }
            });
        });
    }
    function clickToCollectCardInfo(sender, e) {
        var _a;
        return tslib.__awaiter(this, void 0, void 0, function () {
            var layout, info, text;
            return tslib.__generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        layout = (_a = sender.layout) !== null && _a !== void 0 ? _a : sender;
                        info = collectCardInfo(layout);
                        text = "Карточка: " + info.title + "\n" +
                            "Создана: " + info.created + "\n" +
                            "Дата с: " + info.fromDate + "\n" +
                            "Дата по: " + info.toDate + "\n" +
                            "Основание: " + info.reason;
                        return [4 /*yield*/, MessageBox.MessageBox.ShowInfo(text)];
                    case 1:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    }

    var bteh = /*#__PURE__*/Object.freeze({
        __proto__: null,
        checkCardBeforeSave: checkCardBeforeSave,
        checkDateChanged: checkDateChanged,
        clickToCollectCardInfo: clickToCollectCardInfo
    });

    ExtensionManager.extensionManager.registerExtension({
        name: "NGNR3Extension",
        version: "1.0.0",
        globalEventHandlers: [bteh],
        layoutServices: []
    });

}));
//# sourceMappingURL=extension.js.map
