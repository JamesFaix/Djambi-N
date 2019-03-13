import * as React from 'react';
import { Link } from 'react-router-dom';
import Icon, { IconKind } from '../icon';

export interface LinkButtonProps {
    to : string,
    label? : string,
    icon? : IconKind,
    newWindow? : boolean
}

export default class LinkButton extends React.Component<LinkButtonProps> {

    private newWindowOnClick(){
        const win = window.open(this.props.to, '_blank');
        win.focus();
    }

    private getContent() {
        if (this.props.label) {
            return <span>{this.props.label}</span>;
        }

        if (this.props.icon) {
            return <Icon kind={this.props.icon}/>;
        }

        throw "";
    }

    render() {
        if (this.props.newWindow) {
            return (
                <button
                    onClick={() => this.newWindowOnClick()}
                >
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