import * as React from 'react';
import '../../index.css';

export interface HintCellProps {
    text : string
}

export default class HintCell extends React.Component<HintCellProps> {

    render() {
        return  (
            <td className="lightText lobbyPlayersTableTextCell">
                {this.props.text}
            </td>
        );
    }
}