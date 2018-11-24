import React from 'react';
import { Link } from 'react-router-dom';
import '../index.css';

export interface LinkButtonProps {
    to : string,
    label : string
}

export default class LinkButton extends React.Component<LinkButtonProps> {

    render() {
        return (
            <Link to={this.props.to}>
                <button>
                    {this.props.label}
                </button>
            </Link>
        );
    }
}