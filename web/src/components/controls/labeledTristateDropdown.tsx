import * as React from 'react';
import TristateDropdown from './tristateDropdown';
import LabeledControl from './labeledControl';

export interface LabeledTristateDropdownProps {
    label : string,
    onChange(name : string, value : boolean) : void,
    tip ?: string,
    value : boolean
}

export default class LabeledTristateDropdown extends React.Component<LabeledTristateDropdownProps> {
    render() {
        return (
            <LabeledControl
                label={this.props.label}
                tip={this.props.tip}
            >
                <TristateDropdown
                    name={this.props.label}
                    onChange={this.props.onChange}
                    value={this.props.value}
                />
            </LabeledControl>
        );
    }
}