import * as React from 'react';
import { InputTypes } from '../../constants';
import LabeledControl from './labeledControl';

export interface LabeledInputProps {
    type : InputTypes,
    label : string,
    value ? : string,
    onChange(e : React.ChangeEvent<HTMLInputElement>) : void,
    min ? : number,
    max ? : number,
    tip ? : string,
    placeholder ? : string,
    checked ? : boolean
}

export default class LabeledInput extends React.Component<LabeledInputProps> {
    public render() : JSX.Element {
        return (
            <LabeledControl
                label={this.props.label}
                tip={this.props.tip}
            >
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
            </LabeledControl>
        );
    }
}