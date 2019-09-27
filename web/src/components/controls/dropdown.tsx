import * as React from 'react';

export interface DropdownItem<T> {
    value : T,
    label : string
}

export interface DropdownProps<T> {
    items : DropdownItem<T>[],
    onChange(item : T) : void,
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
        this.props.onChange(value);
    }

    render() {
        return (
            <select
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