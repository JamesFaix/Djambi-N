import * as React from 'react';
import ActionButton from '../controls/actionButton';
import { Classes } from '../../styles';

export interface ActionButtonCellProps {
    label : string,
    onClick() : void
}

export default class ActionButtonCell extends React.Component<ActionButtonCellProps> {

    render() {
        return (
            <td className={Classes.centerAligned}>
                <ActionButton
                    label={this.props.label}
                    onClick={this.props.onClick}
                />
            </td>
        );
    }
}