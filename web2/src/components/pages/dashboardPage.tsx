import * as React from 'react';
import * as Redirects from '../redirects';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';
import { connect } from 'react-redux';
import { GamesQuery, Game } from '../../api/model';
import GamesQueryFilters from '../gamesQuery/gamesQueryFilters';
import GamesQueryResultsTable from '../gamesQuery/gamesQueryResultsTable';
import { AppState } from '../../store/state';
import { navigateTo } from '../../history';
import Routes from '../../routes';

interface DashboardPageProps {
    gamesQuery : GamesQuery,
    gamesResults : Game[],
    onSearchClicked : (query: GamesQuery) => void
}

class dashboardPage extends React.Component<DashboardPageProps>{

    render() {
        return (
            <div>
                <Redirects.ToHomeIfNoSession/>
                <button
                    onClick={() => navigateTo(Routes.createGame)}
                >
                    Create game
                </button>
                <GamesQueryFilters/>
                <button
                    onClick={() => this.props.onSearchClicked(this.props.gamesQuery)}
                >
                    Search
                </button>
                <GamesQueryResultsTable/>
            </div>
        );
    }
}

const mapStateToProps = (state: AppState) => {
    if (state.gamesQuery){
        return {
            gamesQuery: state.gamesQuery.query,
            gamesResults: state.gamesQuery.results
        };
    } else {
        return {
            gamesQuery: null,
            gamesResults: []
        };
    }
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onSearchClicked: (query: GamesQuery) => ThunkActions.queryGames(query)(dispatch)
    };
};

const DashboardPage = connect(mapStateToProps, mapDispatchToProps)(dashboardPage);

export default DashboardPage;