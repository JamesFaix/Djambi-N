import * as React from 'react';
import { Game } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Classes } from '../../styles/styles';
import IconBox from '../controls/iconBox';
import { Icons } from '../../utilities/icons';
import Copy from '../../utilities/copy';
import Controller from '../../controller';
import Routes from '../../routes';

interface GamesSearchResultsTableProps {
    games : Game[]
}

class gamesSearchResultsTable extends React.Component<GamesSearchResultsTableProps> {
    render() {
        return (<>
            <SectionHeader text="Results"/>
            <table className={Classes.stripedTable}>
                <tbody>
                    <tr>
                        <th></th>
                        <th>ID</th>
                        <th>Description</th>
                        <th>Created by</th>
                        <th>Status</th>
                        <th># Players</th>
                        <th># Regions</th>
                        <th>Is public</th>
                        <th>Allow guests</th>
                    </tr>
                    {this.props.games
                        .map((g, i) =>
                            <GameRow
                                key={i}
                                game={g}
                            />
                        )
                    }
                </tbody>
            </table>
        </>);
    }
}

interface GameRowProps {
    game : Game
}

const GameRow : React.SFC<GameRowProps> = props => {
    const game = props.game;
    return (
        <tr>
            <td>
                <IconButton
                    icon={Icons.UserActions.loadGame}
                    onClick={() => Controller.navigateTo(Routes.game(game.id))}
                />
            </td>
            <td className={Classes.centered}>
                {game.id}
            </td>
            <td>
                {game.parameters.description}
            </td>
            <td>
                {game.createdBy.userName}
            </td>
            <td className={Classes.centered}>
                <IconBox icon={Icons.gameStatus(game.status)}/>
            </td>
            <td className={Classes.centered}>
                {game.players.length}
            </td>
            <td className={Classes.centered}>
                {game.parameters.regionCount}
            </td>
            <td className={Classes.centered}>
                {Copy.boolToYesOrNo(game.parameters.isPublic)}
            </td>
            <td className={Classes.centered}>
                {Copy.boolToYesOrNo(game.parameters.allowGuests)}
            </td>
        </tr>
    );
}

const mapStateToProps = (state : State) => {
    if (state.gamesQuery && state.gamesQuery.results) {
        return {
            games: state.gamesQuery.results
        };
    } else {
        return {
            games : []
        };
    }
};

const GamesSearchResultsTable = connect(mapStateToProps)(gamesSearchResultsTable);
export default GamesSearchResultsTable;