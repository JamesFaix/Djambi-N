import * as React from 'react';
import Dropdown, { DropdownItem } from './dropdown';

export interface EnumDropdownProps<TEnum> {
    name : string,
    onChange(name : string, value : TEnum) : void,
    value : TEnum
    enum : object,
    getLabel? : (value:TEnum) => string,
    includeNull? : boolean
}

export default class EnumDropdown<TEnum> extends React.Component<EnumDropdownProps<TEnum>> {

    private getLabel(value : TEnum) : string {
        if (this.props.getLabel) {
            return this.props.getLabel(value);
        } else {
            return value === null ? "(Any)" : value.toString();
        }
    }

    private getItems() : DropdownItem<TEnum>[] {
        const values : TEnum[] = [];
        if (this.props.includeNull !== false) {
            values.push(null);
        }

        const enumValues = Object.keys(this.props.enum)
            .map(c => (this.props.enum as any)[c] as TEnum);
        values.push(...enumValues);

        return values.map(v => {
            return {
                value: v,
                label: this.getLabel(v)
            };
        });
    }

    render() {
        return (
            <Dropdown
                name={this.props.name}
                onChange={(name, value) => this.props.onChange(name, value)}
                currentValue={this.props.value}
                items={this.getItems()}
            />
        );
    }
}