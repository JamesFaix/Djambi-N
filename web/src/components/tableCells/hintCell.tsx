import * as React from 'react';
import { Kernel as K } from '../../kernel';

export interface HintCellProps {
    text : string,
    noBorder? : boolean
}

export default class HintCell extends React.Component<HintCellProps> {

    render() {
        const classes = [K.classes.lightText, K.classes.paddedCell];
        if (this.props.noBorder) {
            classes.push(K.classes.borderless);
        }
        return  (
            <td className={K.classes.combine(classes)}>
                {this.props.text}
            </td>
        );
    }
}