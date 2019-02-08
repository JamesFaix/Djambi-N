import * as React from 'react';
import { Classes } from '../../styles';

export interface HintCellProps {
    text : string,
    noBorder? : boolean
}

export default class HintCell extends React.Component<HintCellProps> {

    render() {
        const classes = [Classes.lightText, Classes.paddedCell];
        if (this.props.noBorder) {
            classes.push(Classes.borderless);
        }
        return  (
            <td className={Classes.combine(classes)}>
                {this.props.text}
            </td>
        );
    }
}