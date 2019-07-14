import "./index.css";

export class Classes {

    public readonly appTitle = "appTitle";

    public readonly appTitleCharA = "appTitleCharA";

    public readonly appTitleCharB = "appTitleCharB";

    public readonly pageTitle = "pageTitle";

    public readonly topMenu = "topMenu";

    public readonly centerAligned = "centerAligned";

    public readonly labeledTextBox = "labeledTextBox";

    public readonly thinBorder = "thinBorder";

    public readonly paddedCell = "paddedCell";

    public readonly indented = "indented";

    public readonly lightText = "lightText";

    public readonly table = "table";

    public readonly form = "form";

    public readonly borderless = "borderless";

    public readonly rightAligned = "rightAligned";

    public readonly fullWidth = "fullWidth";

    public readonly flex = "flex";

    public combine(classes: string[]) : string {
        return classes.join(" ");
    }
}

export class Styles {

    public playerGlow(color : string) : React.CSSProperties {
        return { boxShadow: "inset 0 0 3px 3px " + color };
    }

    public readonly noMargin : React.CSSProperties = { margin: "0 auto" };

    public width(width : string) : React.CSSProperties {
        return { width: width };
    }

    public height(height : string) : React.CSSProperties {
        return { height: height };
    }

    public lineHeight(height : string) : React.CSSProperties {
        return { lineHeight: height };
    }

    public readonly noPadding : React.CSSProperties = { padding: 0 };

    public background(color : string) : React.CSSProperties {
        return { background: color };
    }

    public margin(margin : string) : React.CSSProperties {
        return { margin: margin };
    }

    public padding(padding : string) : React.CSSProperties {
        return { padding : padding };
    }

    public padLeft(padding : string) : React.CSSProperties {
        return { paddingRight : padding };
    }

    public padRight(padding : string) : React.CSSProperties {
        return { paddingRight : padding };
    }

    public grid() : React.CSSProperties {
        return { display: "grid" };
    }

    public bold() : React.CSSProperties {
        return { fontWeight: "bold" };
    }

    public fontSize(pixels : number) : React.CSSProperties {
        return { fontSize: pixels + "px" };
    }

    public combine(styles : React.CSSProperties[]) : React.CSSProperties {
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

    public flex(portion : number) : React.CSSProperties {
        return { flex: portion };
    }
}