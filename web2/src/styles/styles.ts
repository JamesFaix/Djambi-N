import { CSSProperties } from 'react';
import Colors from '../utilities/colors';

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

    static playerBoxGlow(playerColorId : number) : CSSProperties {
        const color = Colors.getColorFromPlayerColorId(playerColorId);
        return {
            boxShadow: `inset 0 0 0 3px ${color}`,
            padding: "5px"
        };
    }

    static box() : CSSProperties {
        return {
            borderStyle:"solid",
            borderWidth:1,
            borderColor:"gainsboro",
            padding: "5px"
        };
    }

    static iconButton() : CSSProperties {
        return {
            backgroundColor: "white",
            color: "black"
        };
    }
}