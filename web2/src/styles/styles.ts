import { CSSProperties } from 'react';

export default class Styles {
    static pageContainer() : CSSProperties {
        return {
            display: "flex",
            alignItems: "center",
            flexDirection: "column"
        };
    }

    static pageContainerSpacer() : CSSProperties {
        return {
            height: "20px"
        };
    }

    static smallTopMargin() : CSSProperties {
        return {
            marginTop: "10px"
        };
    }

    static noBorder() : CSSProperties {
        return {
            borderStyle: "none"
        };
    }
}