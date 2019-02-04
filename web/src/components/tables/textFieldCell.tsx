import * as React from 'react';
import '../../index.css';
import StyleService from '../../styleService';

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
                    className={StyleService.classFullWidth}
                />
            </td>
        );
    }
}