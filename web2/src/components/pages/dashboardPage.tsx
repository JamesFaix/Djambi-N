import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { GamesQuery, Game } from '../../api/model';
import GamesSearchResultsTable from '../tables/gamesSearchResultsTable';
import { State } from '../../store/root';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import GamesSearchForm from '../forms/gamesSearchForm';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { Classes } from '../../styles/styles';
import ApiActions from '../../apiActions';

interface DashboardPageProps {
    gamesQuery : GamesQuery,
    gamesResults : Game[],
    onSearchClicked : (query: GamesQuery) => void
}

class dashboardPage extends React.Component<DashboardPageProps>{
    render() {
        return (
            <div className={Classes.pageContainer}>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={{enableCreateGame: true}}/>
                <div className={Classes.pageContainerSpacer}></div>
                <GamesSearchForm/>
                <div className={Classes.pageContainerSpacer}></div>
                <GamesSearchResultsTable/>
            </div>
        );
    }
}

const mapStateToProps = (state: State) => {
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
        onSearchClicked: (query: GamesQuery) => ApiActions.queryGames(query)(dispatch)
    };
};

const DashboardPage = connect(mapStateToProps, mapDispatchToProps)(dashboardPage);

export default DashboardPage;