import * as React from 'react';
import Dropdown from './dropdown';

export interface TristateDropdownProps {
    name : string,
    onChange(name : string, value : boolean) : void,
    value : boolean
}

export default class TristateDropdown extends React.Component<TristateDropdownProps> {

    private static readonly items : [string, boolean][] =
        [
            ["(Any)", null],
            ["Yes", true],
            ["No", false]
        ];

    render() {
        return (
            <Dropdown
                name={this.props.name}
                onChange={(name, value) => this.props.onChange(name, value)}
                currentValue={this.props.value}
                items={TristateDropdown.items}
            />
        );
    }
}