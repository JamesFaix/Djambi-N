import * as React from 'react';
import { GameParameters, GameStatus } from "../../api/model";
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { SectionHeader } from '../controls/headers';
import { Classes } from '../../styles/styles';
import Copy from '../../utilities/copy';

interface GameParametersTableProps {
    parameters : GameParameters,
    status : GameStatus
}

const gameParametersTable : React.SFC<GameParametersTableProps> = props => {
    if (!props.parameters) {
        return null;
    }

    return (<>
        <SectionHeader text="Game settings"/>
        <table>
            <tbody>
                <tr>
                    <td>Description</td>
                    <td>{props.parameters.description}</td>
                </tr>
                <tr>
                    <td>Regions</td>
                    <td className={Classes.centered}>
                        {props.parameters.regionCount}
                    </td>
                </tr>
                <tr>
                    <td>Allow guests</td>
                    <td className={Classes.centered}>
                        {Copy.boolToYesOrNo(props.parameters.allowGuests)}
                    </td>
                </tr>
                <tr>
                    <td>Public</td>
                    <td className={Classes.centered}>
                        {Copy.boolToYesOrNo(props.parameters.isPublic)}
                    </td>
                </tr>
                <tr>
                    <td>Status</td>
                    <td className={Classes.centered}>
                        {props.status}
                    </td>
                </tr>
            </tbody>
        </table>
    </>);
}

const mapStateToProps = (state : State) => {
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
