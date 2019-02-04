import * as React from 'react';
import '../../index.css';
import StyleService from '../../styleService';

export interface HintCellProps {
    text : string
}

export default class HintCell extends React.Component<HintCellProps> {

    render() {
        const className = StyleService.classes([StyleService.classLightText, StyleService.classBorderless]);
        return  (
            <td className={className}>
                {this.props.text}
            </td>
        );
    }
}