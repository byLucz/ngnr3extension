import { BaseControl } from "@docsvision/webclient/System/BaseControl";
import { BaseControlImpl } from "@docsvision/webclient/System/BaseControlImpl";
import { AviasalesControlParams, AviasalesControlState, AviasalesControlImpl } from "./AviasalesControlImpl";

export class IAviasalesControl extends BaseControl<AviasalesControlParams, AviasalesControlState> {
    protected createParams(): AviasalesControlParams {
        return new AviasalesControlParams();
    }

    protected createImpl(): BaseControlImpl<AviasalesControlParams, AviasalesControlState> {
        return new AviasalesControlImpl(this.params, this.state);
    }
}
