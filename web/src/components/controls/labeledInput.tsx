import * as React from 'react';
import '../../index.css';
import { InputTypes } from '../../constants';

export interface LabeledInputProps {
    type : InputTypes,
    label : string,
    value? : string,
    onChange(e : React.ChangeEvent<HTMLInputElement>) : void
    min? : number,
    max? : number,
    tip? : string,
    placeholder? : string
    checked? : boolean
}

export default class LabeledInput extends React.Component<LabeledInputProps> {
    render() {
        return (
            <div
                className="labeledTextBox"
                title={this.props.tip}
            >
                <label>{this.props.label}:</label>
                <input
                    name={this.props.label}
                    type={this.props.type}
                    value={this.props.value}
                    onChange={this.props.onChange}
                    min={this.props.min}
                    max={this.props.max}
                    checked={this.props.checked}
                    placeholder={this.props.placeholder}
                />
            </div>
        );
    }
}