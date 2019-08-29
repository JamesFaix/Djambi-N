export class Classes {
    static readonly pageContainer = "pageContainer";

    static readonly narrowContainer = "narrowContainer";

    static readonly containerSize = "containerSize";

    static readonly verticalSpacerLarge = "verticalSpacerLarge";

    static readonly verticalSpacerSmall = "verticalSpacerSmall";

    static readonly stripedTable = "stripedTable";

    static readonly centered = "centered";

    static readonly indented = "indented";

    static readonly box = "box";

    static readonly playerBox = "playerBox";

    static readonly thinBorder = "thinBorder";

    static readonly iconBox = "iconBox";
}

export class Styles {
    static readonly timelineBar : React.CSSProperties = {
        height: "100%",
        width: "400px",
        justifyContent: "space-between",
        position: "fixed",
        right: 0
    }

    static readonly timelineBarSection : React.CSSProperties = {
        flex: 0,
        width: "100%"
    }

    static readonly timelineBarHistorySection : React.CSSProperties = {
        flex: 1,
        width: "100%"
    }

    static readonly turnCycleSection : React.CSSProperties = {
        display: "flex"
    }

    static readonly historyContainer : React.CSSProperties = {
        display: "flex",
        flexDirection: "column"
    }

    static readonly topBar : React.CSSProperties = {
        height: "50px",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center"
    }

    static readonly topBarNavigation : React.CSSProperties = {
        flex: 1,
        textAlign: "left"
    }

    static readonly topBarTitle : React.CSSProperties = {
        flex: 1,
        textAlign: "center"
    }

    static readonly topBarUser : React.CSSProperties = {
        flex: 1,
        textAlign: "right"
    }
}