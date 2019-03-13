import * as React from 'react';

export enum IconKind {
    Commit,
    Reset,
    AcceptDraw,
    RevokeDraw,
    Concede,
    Snapshots,
    Save,
    Load,
    Delete,
    Find,
    Enter,
    New,
    Home,
    Rules,
    MyGames,
    Logout,
    Start
}

export interface IconnProps {
    kind : IconKind
}

export default class Icon extends React.Component<IconnProps> {

    private getIconName(kind : IconKind) {
        switch (kind) {
            case IconKind.Commit:
                return "check";
            case IconKind.Reset:
                return "undo";
            case IconKind.AcceptDraw:
                return "handshake";
            case IconKind.RevokeDraw:
                return "hand-middle-finger";
            case IconKind.Concede:
                return "flag";
            case IconKind.Snapshots:
                return "camera";
            case IconKind.Save:
                return "save";
            case IconKind.Load:
                return "folder-open";
            case IconKind.Delete:
                return "trash-alt";
            case IconKind.Find:
                return "search";
            case IconKind.Enter:
                return "door-open";
            case IconKind.New:
                return "plus";
            case IconKind.Home:
                return "home";
            case IconKind.Rules:
                return "scroll";
            case IconKind.MyGames:
                return "inbox";
            case IconKind.Logout:
                return "sign-out-alt";
            case IconKind.Start:
                return "play";
            default:
                throw "Unsupported icon.";
        }
    }

    render() {
        const className = "fas fa-" + this.getIconName(this.props.kind);
        return <i className={className}></i>;
    }
}