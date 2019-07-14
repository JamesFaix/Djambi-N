import * as React from 'react';
import { Kernel as K } from '../../kernel';

export interface EmphasizedTextCellProps {
    text : string,
    noBorder ?: boolean
}

export default class EmphasizedTextCell extends React.Component<EmphasizedTextCellProps> {

    render() {
        const classes = [K.classes.paddedCell];
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