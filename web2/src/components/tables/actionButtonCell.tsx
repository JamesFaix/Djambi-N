import * as React from 'react';
import '../../index.css';
import ActionButton from '../actionButton';

export interface ActionButtonCellProps {
    label : string,
    onClick() : void
}

export default class ActionButtonCell extends React.Component<ActionButtonCellProps> {

    render() {
        return (
            <td className="centeredContainer">
                <ActionButton
                    label={this.props.label}
                    onClick={this.props.onClick}
                />
            </td>
        );
    }
}