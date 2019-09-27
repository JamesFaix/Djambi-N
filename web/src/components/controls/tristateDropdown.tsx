import * as React from 'react';
import Dropdown from './dropdown';

export interface TristateDropdownProps {
    onChange(value : boolean) : void,
    value : boolean,
    style ?: React.CSSProperties
}

export default class TristateDropdown extends React.Component<TristateDropdownProps> {

    private static readonly items =
    [
        {
            label: "(Any)",
            value: null
        },
        {
            label: "Yes",
            value: true
        },
        {
            label: "No",
            value: false
        }
    ];

    render() {
        return (
            <Dropdown
                onChange={(value) => this.props.onChange(value)}
                currentValue={this.props.value}
                items={TristateDropdown.items}
                style={this.props.style}
            />
        );
    }
}