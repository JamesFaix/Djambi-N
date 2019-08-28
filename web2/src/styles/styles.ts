import { CSSProperties } from 'react';
export default class Styles {
    static smallTopMargin() : CSSProperties {
        return {
            marginTop: "10px"
        };
    }
}

export class Classes {
    static iconButton(mouseover : boolean) {
        return mouseover ? "iconButtonMouseover" : "iconButton";
    }

    static readonly pageContainer = "pageContainer";

    static readonly pageContainerSpacer = "pageContainerSpacer";

    static readonly borderlessTable = "borderlessTable";

    static readonly box = "box";

    static readonly playerBox = "playerBox";
}