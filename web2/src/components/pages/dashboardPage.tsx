import * as React from 'react';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';
import { connect } from 'react-redux';
import { GamesQuery, Game } from '../../api/model';
import GamesSearchResultsTable from '../tables/gamesSearchResultsTable';
import { AppState } from '../../store/state';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import GamesSearchForm from '../forms/gamesSearchForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';

interface DashboardPageProps {
    gamesQuery : GamesQuery,
    gamesResults : Game[],
    onSearchClicked : (query: GamesQuery) => void
}

class dashboardPage extends React.Component<DashboardPageProps>{
    render() {
        return (
            <div style={Styles.pageContainer()}>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={{enableCreateGame: true}}/>
                <div style={Styles.pageContainerSpacer()}></div>
                <GamesSearchForm/>
                <div style={Styles.pageContainerSpacer()}></div>
                <GamesSearchResultsTable/>
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