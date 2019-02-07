import * as React from 'react';

export interface TextCellProps {
    text : string
}

export default class TextCell extends React.Component<TextCellProps> {

    render() {
        return  (
            <td>
                {this.props.text}
            </td>
        );
    }
}