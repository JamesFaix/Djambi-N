import * as React from 'react';

export interface DropdownItem<T> {
    value : T,
    label : string
}

interface DropdownProps<T> {
    name : string,
    items : DropdownItem<T>[],
    onChange(name : string, item : T) : void,
    currentValue : T,
    style ?: React.CSSProperties
}

export default class Dropdown<T> extends React.Component<DropdownProps<T>> {

    private getLabelOfValue(value : T) : string {
        return this.props.items.find(x => x.value === value).label;
    }

    private getValueOfLabel(label : string) : T {
        return this.props.items.find(x => x.label === label).value;
    }

    private onChange(event : React.ChangeEvent<HTMLSelectElement>) {
        const label = event.target.value;
        const value = this.getValueOfLabel(label);
        this.props.onChange(this.props.name, value);
    }

    render() {
        return (
            <select
                name={this.props.name}
                onChange={e => this.onChange(e)}
                value={this.getLabelOfValue(this.props.currentValue)}
                style={this.props.style}
            >
                {this.props.items.map((item, i) =>
                    <option
                        key={"option" + i}
                        value={item.label}
                    >
                        {item.label}
                    </option>
                )}
            </select>
        );
    }
}

interface MultiDropdownProps<T> {
    name : string,
    items : DropdownItem<T>[],
    onChange(name : string, items : T[]) : void,
    currentValues : T[],
    style ?: React.CSSProperties
}