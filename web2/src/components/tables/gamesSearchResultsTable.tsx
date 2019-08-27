import * as React from 'react';
import { Game } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import { boolToYesOrNo } from '../../utilities/copy';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { faDoorOpen } from '@fortawesome/free-solid-svg-icons';
import ApiActions from '../../apiActions';

interface GamesSearchResultsTableProps {
    games : Game[]
}

class gamesSearchResultsTable extends React.Component<GamesSearchResultsTableProps> {
    render() {
        return (
            <div>
                <SectionHeader text="Results"/>
                <table>
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
                    icon={faDoorOpen}
                    onClick={() => ApiActions.navigateToGame(game)}
                />
            </td>
            <td>{game.id}</td>
            <td>{game.parameters.description}</td>
            <td>{game.createdBy.userName}</td>
            <td>{game.status}</td>
            <td>{game.players.length}</td>
            <td>{game.parameters.regionCount}</td>
            <td>{boolToYesOrNo(game.parameters.isPublic)}</td>
            <td>{boolToYesOrNo(game.parameters.allowGuests)}</td>
        </tr>
    );
}

const mapStateToProps = (state : AppState) => {
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