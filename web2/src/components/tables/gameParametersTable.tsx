import * as React from 'react';
import { GameParameters, GameStatus } from "../../api/model";
import { AppState } from '../../store/state';
import { connect } from 'react-redux';

interface GameParametersTableProps {
    parameters : GameParameters,
    status : GameStatus
}

const gameParametersTable : React.SFC<GameParametersTableProps> = props => {
    if (!props.parameters) {
        return null;
    }

    return (
        <div>
            <table>
                <tbody>
                    <tr>
                        <td>Description</td>
                        <td>{props.parameters.description}</td>
                    </tr>
                    <tr>
                        <td>Regions</td>
                        <td>{props.parameters.regionCount}</td>
                    </tr>
                    <tr>
                        <td>Allow guests</td>
                        <td>{props.parameters.allowGuests}</td>
                    </tr>
                    <tr>
                        <td>Public</td>
                        <td>{props.parameters.isPublic}</td>
                    </tr>
                    <tr>
                        <td>Status</td>
                        <td>{props.status}</td>
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
