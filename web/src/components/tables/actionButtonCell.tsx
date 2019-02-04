import * as React from 'react';
import '../../index.css';
import ActionButton from '../controls/actionButton';
import StyleService from '../../styleService';

export interface ActionButtonCellProps {
    label : string,
    onClick() : void
}

export default class ActionButtonCell extends React.Component<ActionButtonCellProps> {

    render() {
        return (
            <td className={StyleService.classCenteredContainer}>
                <ActionButton
                    label={this.props.label}
                    onClick={this.props.onClick}
                />
            </td>
        );
    }
}