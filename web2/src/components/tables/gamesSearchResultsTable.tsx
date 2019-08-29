import * as React from 'react';
import { Game, GameStatus } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { boolToYesOrNo } from '../../utilities/copy';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import ApiActions from '../../apiActions';
import { Classes } from '../../styles/styles';
import Icons from '../../utilities/icons';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import IconBox from '../controls/iconBox';

interface GamesSearchResultsTableProps {
    games : Game[]
}

class gamesSearchResultsTable extends React.Component<GamesSearchResultsTableProps> {
    render() {
        return (
            <div>
                <SectionHeader text="Results"/>
                <table className={Classes.stripedTable}>
                    <tbody>
                        <tr>
                            <th></th>
                            <th>GameID</th>
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
            </div>
        );
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
                    title="Load"
                    icon={Icons.Page.lobby}
                    onClick={() => ApiActions.navigateToGame(game)}
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
                <GameStatusIcon status={game.status}/>
            </td>
            <td className={Classes.centered}>
                {game.players.length}
            </td>
            <td className={Classes.centered}>
                {game.parameters.regionCount}
            </td>
            <td className={Classes.centered}>
                {boolToYesOrNo(game.parameters.isPublic)}
            </td>
            <td className={Classes.centered}>
                {boolToYesOrNo(game.parameters.allowGuests)}
            </td>
        </tr>
    );
}

interface GameStatusIconProps {
    status : GameStatus
}

const GameStatusIcon : React.SFC<GameStatusIconProps> = props => {
    let title : string;
    let icon : IconDefinition;

    switch(props.status) {
        case GameStatus.Canceled:
            title = "Canceled";
            icon = Icons.GameStatus.Canceled;
            break;
        case GameStatus.InProgress:
            title = "In progress";
            icon = Icons.GameStatus.InProgress;
            break;
        case GameStatus.Over:
            title = "Over";
            icon = Icons.GameStatus.Over;
            break;
        case GameStatus.Pending:
            title = "Pending";
            icon = Icons.GameStatus.Pending;
            break;
        default:
            throw "Invalid game status: " + props.status;
    }

    return (
        <IconBox
            title={title}
            icon={icon}
        />
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