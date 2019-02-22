import * as React from 'react';
import { Kernel as K } from '../../kernel';

export interface TextFieldCellProps {
    value : string,
    onChange : (e : React.ChangeEvent<HTMLInputElement>) => void
}

export default class TextFieldCell extends React.Component<TextFieldCellProps> {

    render() {
        return (
            <td>
                <input
                    type="text"
                    value={this.props.value}
                    onChange={this.props.onChange}
                    className={K.classes.fullWidth}
                />
            </td>
        );
    }
}