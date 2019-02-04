import * as React from 'react';
import '../../index.css';
import StyleService from '../../styleService';

export interface EmphasizedTextCellProps {
    text : string
}

export default class EmphasizedTextCell extends React.Component<EmphasizedTextCellProps> {

    render() {
        return  (
            <td className={StyleService.classPaddedCell}>
                {this.props.text}
            </td>
        );
    }
}