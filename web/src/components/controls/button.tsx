import * as React from 'react';
import Icon, { IconKind } from '../icon';
import { Link } from 'react-router-dom';

export enum ButtonKind {
    Link,
    Action
}

export interface ButtonProps {
    kind : ButtonKind,

    //Content
    label? : string,
    icon? : IconKind,

    //Action button properties
    onClick? () : void,

    //Link button properties
    to?: string,
    newWindow? : boolean
}

export default class Button extends React.Component<ButtonProps> {

    private newWindowOnClick(){
        const win = window.open(this.props.to, '_blank');
        win.focus();
    }

    private getContent() {
        if (this.props.label !== undefined) {
            return <span>{this.props.label}</span>;
        }

        if (this.props.icon !== undefined) {
            return <Icon kind={this.props.icon}/>;
        }

        throw "";
    }

    render() {
        switch (this.props.kind) {
            case ButtonKind.Action:
                return this.renderActionButton();
            case ButtonKind.Link:
                return this.renderLinkButton();
            default:
                throw "";
        }
    }

    private renderActionButton() {
        return (
            <button onClick={_ => this.props.onClick()}>
                {this.getContent()}
            </button>
        );
    }

    private renderLinkButton() {
        if (this.props.newWindow) {
            return (
                <button onClick={() => this.newWindowOnClick()}>
                    {this.getContent()}
                </button>
            );
        } else {
            return (
                <Link to={this.props.to}>
                    <button>
                        {this.getContent()}
                    </button>
                </Link>
            );
        }
    }
}