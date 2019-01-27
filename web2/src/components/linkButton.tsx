import * as React from 'react';
import { Link } from 'react-router-dom';
import '../index.css';

export interface LinkButtonProps {
    to : string,
    label : string,
    newWindow? : boolean
}

export default class LinkButton extends React.Component<LinkButtonProps> {

    private newWindowOnClick(){
        const win = window.open(this.props.to, '_blank');
        win.focus();
    }

    render() {
        if (this.props.newWindow) {
            return (
                <button
                    onClick={() => this.newWindowOnClick()}
                >
                    {this.props.label}
                </button>
            );
        } else {
            return (
                <Link to={this.props.to}>
                    <button>
                        {this.props.label}
                    </button>
                </Link>
            );
        }
    }
}