export default class StyleService {

//---Classes---

    public static readonly classAppTitle = "appTitle";

    public static readonly classAppTitleCharA = "appTitleCharA";

    public static readonly classAppTitleCharB = "appTitleCharB";

    public static readonly classPageTitle = "pageTitle";

    public static readonly classTopMenu = "topMenu";

    public static readonly classCenteredContainer = "centeredContainer";

    public static readonly classLabeledTextBox = "labeledTextBox";

    public static readonly classThinBorder = "thinBorder";

    public static readonly classPaddedCell = "paddedCell";

    public static readonly classIndented = "indented";

    public static readonly classLightText = "lightText";

    public static readonly classTable = "table";

    public static readonly classForm = "form";

    public static readonly classBorderless = "borderless";

    public static readonly classRightAligned = "rightAligned";

    public static readonly classLobbyDetailsContainer = "lobbyDetailsContainer";

    public static readonly classFullWidth = "fullWidth";

    public static classes(classes: string[]) : string {
        return classes.join(",");
    }

//---Dynamic styles---

    public static styleCurrentTurnPanelGlow(color : string) : React.CSSProperties {
        return {
            boxShadow: "inset 0 0 5px 5px " + color,
            width: "40%"
        };
    }

    public static stylePlayersPanelGlow(color : string) : React.CSSProperties {
        return {
            boxShadow: "inset 0 0 5px 5px " + color
        };
    }

    public static styleFlexContainer() : React.CSSProperties {
        return {
            display:"flex"
        };
    }

    public static styleHistoryTable() : React.CSSProperties {
        return {
            width:"100%"
        };
    }

    public static styleGamePageContainer(canvasSize : number) : React.CSSProperties {
        return {
            margin: "0 auto",
            width: canvasSize
        };
    }

    public static styleGamePageCanvas(canvasSize : number) : React.CSSProperties {
        return {
            margin: "0 auto",
            width: canvasSize,
            height: canvasSize
        };
    }

    public static styleWidth(width : number) : React.CSSProperties {
        return {
            width: width + "%"
        }
    }

    public static noPadding() : React.CSSProperties {
        return {
            padding: 0
        };
    }

    public static turnCycleElement(color : string, scale : number) : React.CSSProperties {
        return {
            background: color,
            height: scale,
            width: scale
        };
    }
}