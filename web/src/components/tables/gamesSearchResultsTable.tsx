import * as React from 'react';
import { Game } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { SectionHeader } from '../controls/headers';
import { Classes } from '../../styles/styles';
import GameSearchResultsRow from './gameSearchResultsRow';

class gamesSearchResultsTable extends React.Component<{
    games : Game[]
}> {
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
                            <GameSearchResultsRow
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