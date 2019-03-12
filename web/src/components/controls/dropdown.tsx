import * as React from 'react';

export interface DropdownProps<T> {
    name : string,
    items : [string, T][],
    onChange(name : string, item : T) : void,
    currentValue : T
}

export default class Dropdown<T> extends React.Component<DropdownProps<T>> {

    private getLabelOfValue(value : T) : string {
        return this.props.items.find(x => x[1] === value)[0];
    }

    private getValueOfLabel(label : string) : T {
        return this.props.items.find(x => x[0] === label)[1];
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
            >
            {
                this.props.items.map((item, i) =>
                    <option
                        key={"option" + i}
                        value={item[0]}
                    >
                    {item[0]}
                    </option>
                )
            }
            </select>
        );
    }
}