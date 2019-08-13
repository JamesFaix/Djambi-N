import * as React from 'react';
import { GameParameters, GameStatus } from "../../api/model";
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import { boolToYesOrNo } from '../../utilities/copy';
import SectionHeader from '../sections/sectionHeader';

interface GameParametersTableProps {
    parameters : GameParameters,
    status : GameStatus
}

const gameParametersTable : React.SFC<GameParametersTableProps> = props => {
    if (!props.parameters) {
        return null;
    }

    const cellStyle = {
        borderStyle:"none"
    };

    const centeredCellStyle : React.CSSProperties = {
        borderStyle: "none",
        textAlign: "center"
    }

    return (
        <div>
            <SectionHeader text="Game settings"/>
            <table>
                <tbody>
                    <tr>
                        <td style={cellStyle}>Description</td>
                        <td style={cellStyle}>{props.parameters.description}</td>
                    </tr>
                    <tr>
                        <td style={cellStyle}>Regions</td>
                        <td style={centeredCellStyle}>{props.parameters.regionCount}</td>
                    </tr>
                    <tr>
                        <td style={cellStyle}>Allow guests</td>
                        <td style={centeredCellStyle}>{boolToYesOrNo(props.parameters.allowGuests)}</td>
                    </tr>
                    <tr>
                        <td style={cellStyle}>Public</td>
                        <td style={centeredCellStyle}>{boolToYesOrNo(props.parameters.isPublic)}</td>
                    </tr>
                    <tr>
                        <td style={cellStyle}>Status</td>
                        <td style={centeredCellStyle}>{props.status}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
}

const mapStateToProps = (state : AppState) => {
    if (state.activeGame.game) {
        return {
            parameters: state.activeGame.game.parameters,
            status: state.activeGame.game.status
        };
    } else {
        return {
            parameters: null,
            status: null
        };
    }
};

const GameParametersTable = connect(mapStateToProps)(gameParametersTable);

export default GameParametersTable;
