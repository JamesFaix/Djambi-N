import * as React from 'react';
import { Link } from 'react-router-dom';
import '../index.css';

export interface ActionButtonProps {
    label : string,
    action() : void
}

export default class ActionButton extends React.Component<ActionButtonProps> {

    render() {
        return (
            <button onClick={_ => this.props.action()}>
                {this.props.label}
            </button>
        );
    }
}