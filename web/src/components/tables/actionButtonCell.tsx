import * as React from 'react';
import ActionButton from '../controls/actionButton';
import { Kernel as K } from '../../kernel';

export interface ActionButtonCellProps {
    label : string,
    onClick() : void
}

export default class ActionButtonCell extends React.Component<ActionButtonCellProps> {

    render() {
        return (
            <td className={K.classes.centerAligned}>
                <ActionButton
                    label={this.props.label}
                    onClick={this.props.onClick}
                />
            </td>
        );
    }
}