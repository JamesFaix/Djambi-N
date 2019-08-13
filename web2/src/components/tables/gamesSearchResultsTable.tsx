import * as React from 'react';
import { Game } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';
import { boolToYesOrNo } from '../../utilities/copy';
import { SectionHeader } from '../controls/headers';

interface GamesSearchResultsTableProps {
    games : Game[],
    onViewGameClicked: (game: Game) => void
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
                            .map((g, i) => this.renderRow(g, i))
                        }
                    </tbody>
                </table>
            </div>
        );
    }

    private renderRow(game: Game, rowNumber: number) {
        return (
            <tr key={rowNumber}>
                <td>
                    <button
                        onClick={e => this.props.onViewGameClicked(game)}
                    >
                        Load
                    </button>
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

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onViewGameClicked: (game: Game) => ThunkActions.navigateToGame(game)
    };
};

const GamesSearchResultsTable = connect(mapStateToProps, mapDispatchToProps)(gamesSearchResultsTable);

export default GamesSearchResultsTable;