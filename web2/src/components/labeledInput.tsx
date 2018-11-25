import * as React from 'react';
import '../index.css';

export interface LabeledInputProps {
    type : string,
    label : string,
    value? : string,
    handleChange(e : React.ChangeEvent<HTMLInputElement>) : void
    min? : number,
    max? : number,
    tip? : string,
    placeholder? : string
    checked? : boolean
}

export default class LabeledInput extends React.Component<LabeledInputProps> {
    render() {
        const type = this.props.type ? this.props.type : "text";

        return (
            <div
                className="labeledTextBox"
                title={this.props.tip}
            >
                <label>{this.props.label}:</label>
                <input
                    name={this.props.label}
                    type={type}
                    value={this.props.value}
                    onChange={this.props.handleChange}
                    min={this.props.min}
                    max={this.props.max}
                    checked={this.props.checked}
                    placeholder={this.props.placeholder}
                />
            </div>
        );
    }
}