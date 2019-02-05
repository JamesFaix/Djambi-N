import * as React from 'react';
import '../../index.css';
import { Classes } from '../../styles';

export interface HintCellProps {
    text : string
}

export default class HintCell extends React.Component<HintCellProps> {

    render() {
        const className = Classes.combine([Classes.lightText, Classes.paddedCell]);
        return  (
            <td className={className}>
                {this.props.text}
            </td>
        );
    }
}