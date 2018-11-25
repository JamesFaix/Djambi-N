import * as React from 'react';
import '../index.css';

export interface LabeledTextBoxProps {
    type : string,
    label : string,
    value : string,
    handleChange(e : React.ChangeEvent<HTMLInputElement>) : void
}

export default class LabeledTextbox extends React.Component<LabeledTextBoxProps> {
    render() {
        const type = this.props.type ? this.props.type : "text";

        return (
            <div className="labeledTextBox">
                <label>{this.props.label}:</label>
                <input
                    name={this.props.label}
                    type={type}
                    value={this.props.value}
                    onChange={this.props.handleChange}
                />
            </div>
        );
    }
}