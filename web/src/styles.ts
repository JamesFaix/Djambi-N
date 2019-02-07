import "./index.css";

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

    public static playerGlow(color : string) : React.CSSProperties {
        return { boxShadow: "inset 0 0 5px 5px " + color };
    }

    public static readonly noMargin : React.CSSProperties = { margin: "0 auto" };

    public static width(width : string) : React.CSSProperties {
        return { width: width };
    }

    public static height(height : string) : React.CSSProperties {
        return { height: height };
    }

    public static readonly noPadding : React.CSSProperties = { padding: 0 };

    public static background(color : string) : React.CSSProperties {
        return { background: color };
    }

    public static combine(styles : React.CSSProperties[]) : React.CSSProperties {
        const len = styles.length;
        switch (len) {
            case 0: return {};
            case 1: return styles[0];
            default:
                let result = {};
                styles.forEach(s => {
                    result = { ...result, ...s };
                })
                return result;
        }
    }
}