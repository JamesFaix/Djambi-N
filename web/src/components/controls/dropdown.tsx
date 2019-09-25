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

export class MultiDropdown<T> extends React.Component<MultiDropdownProps<T>> {
    private readonly selectRef : React.MutableRefObject<HTMLSelectElement>;

    constructor(props : MultiDropdownProps<T>) {
        super(props);
        this.selectRef = React.createRef();
    }

    private onChange(_ : React.ChangeEvent<HTMLSelectElement>) {
        const select = this.selectRef.current;
        const selections : string[] = [];
        for (let i=0; i<select.options.length; i++) {
            let o = select.options[i];
            if (o.selected) {
                selections.push(o.text);
            }
        }
        const values = selections.map(s =>
            this.props.items.find(i => i.label === s).value);

        this.props.onChange(this.props.name, values);
    }

    render() {
        return (
            <select
                ref={this.selectRef}
                name={this.props.name}
                onChange={e => this.onChange(e)}
                style={this.props.style}
                multiple
            >
                {this.props.items.map((item, i) =>
                    <option
                        key={"option" + i}
                        value={item.label}
                        selected={this.props.currentValues.includes(item.value)}
                    >
                        {item.label}
                    </option>
                )}
            </select>
        );
    }
}