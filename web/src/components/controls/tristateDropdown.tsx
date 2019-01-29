import * as React from 'react';
import '../../index.css';

export interface TristateDropdownProps {
    name : string,
    onChange(name : string, value : boolean) : void,
    value : boolean
}

export default class TristateDropdown extends React.Component<TristateDropdownProps> {

    private valueToString(value : boolean) : string {
        switch (value)
        {
            case true: return "Yes";
            case false: return "No";
            case null: return "(Any)";
            default: return undefined;
        }
    }

    private stringToValue(str : string) : boolean {
        switch (str)
        {
            case "Yes": return true;
            case "No": return false;
            case "(Any)": return null;
            default: return undefined;
        }
    }

    private onChange(event : React.ChangeEvent<HTMLSelectElement>) {
        const str = event.target.value;
        const value = this.stringToValue(str);
        this.props.onChange(this.props.name, value);
    }

    render() {
        return (
            <select
                name={this.props.name}
                onChange={e => this.onChange(e)}
                value={this.valueToString(this.props.value)}
            >
                <option value="(Any)">(Any)</option>
                <option value="Yes">Yes</option>
                <option value="No">No</option>
            </select>
        );
    }
}