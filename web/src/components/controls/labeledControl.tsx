import * as React from 'react';
import { Kernel as K } from '../../kernel';

export interface LabeledControlProps {
    label : string,
    tip ? : string
}

export default class LabeledControl extends React.Component<LabeledControlProps> {
    public render() : JSX.Element {
        return (
            <div
                className={K.classes.labeledTextBox}
                title={this.props.tip}
            >
                <label>{this.props.label}:</label>
                {this.props.children}
            </div>
        );
    }
}