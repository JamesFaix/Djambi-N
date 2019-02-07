import * as React from 'react';
import { Classes } from '../../styles';

export interface EmphasizedTextCellProps {
    text : string
}

export default class EmphasizedTextCell extends React.Component<EmphasizedTextCellProps> {

    render() {
        return  (
            <td className={Classes.paddedCell}>
                {this.props.text}
            </td>
        );
    }
}