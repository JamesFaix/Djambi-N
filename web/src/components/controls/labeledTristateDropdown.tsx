import * as React from 'react';
import TristateDropdown from './tristateDropdown';
import { Kernel as K } from '../../kernel';

export interface LabeledTristateDropdownProps {
    label : string,
    onChange(name: string, value : boolean) : void
    tip? : string,
    value: boolean
}

export default class LabeledTristateDropdown extends React.Component<LabeledTristateDropdownProps> {
    render() {
        return (
            <div
                className={K.classes.labeledTextBox}
                title={this.props.tip}
            >
                <label>{this.props.label}:</label>
                <TristateDropdown
                    name={this.props.label}
                    onChange={this.props.onChange}
                    value={this.props.value}
                />
            </div>
        );
    }
}