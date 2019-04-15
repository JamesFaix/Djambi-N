import * as React from 'react';
import { Kernel as K } from '../../kernel';

export enum IconKind {
    //Player status
    AcceptDraw,
    Alive,
    Concede,
    Eliminated,
    RevokeDraw,
    Victorious,

    //Game status
    Canceled,
    InProgress,
    Over,
    Pending, //Also player status

    //Panels & Pages
    Find,
    History,
    Home,
    Login,
    Logout,
    MyGames,
    Players,
    Rules,
    Signup,
    Snapshots,
    TurnCycle,

    //Actions
    Close,
    Collapse,
    Delete,
    Enter,
    Expand,
    Load,
    New,
    Remove,
    Reset,
    Save,
    Submit,

    //Misc
    GuestOf,
}

export interface IconnProps {
    kind : IconKind,
    hint? : string
}

export default class Icon extends React.Component<IconnProps> {

    private getIconName(kind : IconKind) {
        switch (kind) {
            //Player status
            case IconKind.AcceptDraw: return "handshake";
            case IconKind.Alive: return "heart";
            case IconKind.Concede: return "flag";
            case IconKind.Eliminated: return "skull";
            case IconKind.RevokeDraw: return "hand-middle-finger";
            case IconKind.Victorious: return "trophy";

            //Game status
            case IconKind.Canceled: return "ban";
            case IconKind.InProgress: return "play";
            case IconKind.Over: return "award";
            case IconKind.Pending: return "spinner"; //Also player status

            //Pages & Panels
            case IconKind.Find: return "search";
            case IconKind.History: return "list";
            case IconKind.Home: return "home";
            case IconKind.Login: return "sign-in-alt";
            case IconKind.Logout: return "sign-out-alt";
            case IconKind.MyGames: return "inbox";
            case IconKind.Players: return "users";
            case IconKind.Rules: return "scroll";
            case IconKind.Signup: return "user-plus";
            case IconKind.Snapshots: return "camera";
            case IconKind.TurnCycle: return "clock";

            //Actions
            case IconKind.Close: return "times";
            case IconKind.Collapse: return "angle-left";
            case IconKind.Delete: return "trash-alt";
            case IconKind.Enter: return "door-open";
            case IconKind.Expand: return "ellipsis-h";
            case IconKind.Load: return "folder-open";
            case IconKind.New: return "plus";
            case IconKind.Remove: return "minus";
            case IconKind.Reset: return "undo";
            case IconKind.Save: return "save";
            case IconKind.Submit: return "check";

            //Misc
            case IconKind.GuestOf: return "id-badge";

            default: throw "Unsupported icon.";
        }
    }

    public render() : JSX.Element {
        const className = "fas fa-" + this.getIconName(this.props.kind);
        const style = K.styles.fontSize(20);
        return (
            <i
                className={className}
                style={style}
                title={this.props.hint}
            >
            </i>
        );
    }
}