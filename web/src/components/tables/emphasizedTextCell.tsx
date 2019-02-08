import * as React from 'react';
import { Classes } from '../../styles';

export interface EmphasizedTextCellProps {
    text : string,
    noBorder? : boolean
}

export default class EmphasizedTextCell extends React.Component<EmphasizedTextCellProps> {

    render() {
        const classes = [Classes.paddedCell];
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