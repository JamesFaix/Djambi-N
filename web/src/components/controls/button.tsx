import * as React from 'react';
import Icon, { IconKind } from '../icons/icon';
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
    hint?: string,

    //Action button properties
    onClick? () : void,

    //Link button properties
    to?: string,
    newWindow? : boolean
}

export default class Button extends React.Component<ButtonProps> {
    constructor(props : ButtonProps) {
        super(props);

        if (props.label === undefined && props.icon === undefined) {
            throw "ButtonProps must have either an 'icon' or a 'label'.";
        }

        if (props.label !== undefined && props.icon !== undefined) {
            throw "ButtonProps must have either an 'icon' or a 'label', but not both.";
        }

        if (props.kind === ButtonKind.Action && props.onClick === undefined) {
            throw "ButtonProps of kind Action must have an 'onClick' callback.";
        }

        if (props.kind === ButtonKind.Link && props.to === undefined) {
            throw "ButtonProps of kind Link must have a 'to' url.";
        }
    }

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

        return undefined;
    }

    render() {
        switch (this.props.kind) {
            case ButtonKind.Action:
                return this.renderActionButton();
            case ButtonKind.Link:
                return this.props.newWindow
                    ? this.renderExternalLinkButton()
                    : this.renderInternalLinkButton();
            default:
                return undefined;
        }
    }

    private renderActionButton() {
        return (
            <button
                onClick={_ => this.props.onClick()}
                title={this.props.hint}
            >
                {this.getContent()}
            </button>
        );
    }

    private renderInternalLinkButton() {
        return (
            <Link to={this.props.to}>
                <button
                    title={this.props.hint}
                >
                    {this.getContent()}
                </button>
            </Link>
        );
    }

    private renderExternalLinkButton() {
        return (
            <button
                onClick={() => this.newWindowOnClick()}
                title={this.props.hint}
            >
                {this.getContent()}
            </button>
        );
    }
}