export class Classes {

    public static readonly appTitle = "appTitle";

    public static readonly appTitleCharA = "appTitleCharA";

    public static readonly appTitleCharB = "appTitleCharB";

    public static readonly pageTitle = "pageTitle";

    public static readonly topMenu = "topMenu";

    public static readonly centerAligned = "centerAligned";

    public static readonly labeledTextBox = "labeledTextBox";

    public static readonly thinBorder = "thinBorder";

    public static readonly paddedCell = "paddedCell";

    public static readonly indented = "indented";

    public static readonly lightText = "lightText";

    public static readonly table = "table";

    public static readonly form = "form";

    public static readonly borderless = "borderless";

    public static readonly rightAligned = "rightAligned";

    public static readonly fullWidth = "fullWidth";

    public static readonly flex = "flex";

    public static combine(classes: string[]) : string {
        return classes.join(" ");
    }

}

export class Styles {

    public static currentTurnPanelGlow(color : string) : React.CSSProperties {
        return {
            boxShadow: "inset 0 0 5px 5px " + color,
            width: "40%"
        };
    }

    public static playersPanelGlow(color : string) : React.CSSProperties {
        return {
            boxShadow: "inset 0 0 5px 5px " + color
        };
    }

    public static gamePageContainer(canvasSize : number) : React.CSSProperties {
        return {
            margin: "0 auto",
            width: canvasSize
        };
    }

    public static gamePageCanvas(canvasSize : number) : React.CSSProperties {
        return {
            margin: "0 auto",
            width: canvasSize,
            height: canvasSize
        };
    }

    public static width(width : number) : React.CSSProperties {
        return {
            width: width + "%"
        }
    }

    public static readonly noPadding : React.CSSProperties = {
        padding: 0
    };

    public static turnCycleElement(color : string, scale : number) : React.CSSProperties {
        return {
            background: color,
            height: scale,
            width: scale
        };
    }

    public static absoluteHeight(pixels : number) : React.CSSProperties {
        return {
            height: pixels + "px"
        };
    }
}