import * as React from 'react';
import { Kernel as K } from '../kernel';

export enum IconKind {
    AcceptDraw,
    Collapse,
    Concede,
    Delete,
    Enter,
    Expand,
    Find,
    Home,
    Load,
    Login,
    Logout,
    MyGames,
    New,
    Remove,
    Reset,
    RevokeDraw,
    Rules,
    Save,
    Signup,
    Snapshots,
    Start,
    Submit,
}

export interface IconnProps {
    kind : IconKind
}

export default class Icon extends React.Component<IconnProps> {

    private getIconName(kind : IconKind) {
        switch (kind) {
            case IconKind.AcceptDraw: return "handshake";
            case IconKind.Collapse: return "angle-left";
            case IconKind.Concede: return "flag";
            case IconKind.Delete: return "trash-alt";
            case IconKind.Enter: return "door-open";
            case IconKind.Expand: return "ellipsis-h";
            case IconKind.Find: return "search";
            case IconKind.Home: return "home";
            case IconKind.Load: return "folder-open";
            case IconKind.Login: return "sign-in-alt";
            case IconKind.Logout: return "sign-out-alt";
            case IconKind.MyGames: return "inbox";
            case IconKind.New: return "plus";
            case IconKind.Remove: return "minus";
            case IconKind.Reset: return "undo";
            case IconKind.RevokeDraw: return "hand-middle-finger";
            case IconKind.Rules: return "scroll";
            case IconKind.Save: return "save";
            case IconKind.Signup: return "user-plus";
            case IconKind.Snapshots: return "camera";
            case IconKind.Start: return "play";
            case IconKind.Submit: return "check";
            default: throw "Unsupported icon.";
        }
    }

    render() {
        const className = "fas fa-" + this.getIconName(this.props.kind);
        const style = K.styles.fontSize(20);
        return <i className={className} style={style}></i>;
    }
}