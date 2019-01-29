import * as React from 'react';
import '../../index.css';

export interface ActionButtonProps {
    label : string,
    onClick() : void
}

export default class ActionButton extends React.Component<ActionButtonProps> {

    render() {
        return (
            <button onClick={_ => this.props.onClick()}>
                {this.props.label}
            </button>
        );
    }
}