import * as React from 'react';
import { connect } from 'react-redux';
import { GamesQuery, Game } from '../../api/model';
import GamesSearchResultsTable from '../tables/gamesSearchResultsTable';
import { State } from '../../store/root';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import GamesSearchForm from '../forms/gamesSearchForm';
import BasicPageContainer from '../sections/basicPageContainer';
import Controller from '../../controller';

interface DashboardPageProps {
    gamesQuery : GamesQuery,
    gamesResults : Game[],
    onSearchClicked : (query: GamesQuery) => void
}

class dashboardPage extends React.Component<DashboardPageProps>{
    render() {
        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <GamesSearchForm/>
                <br/>
                <br/>
                <GamesSearchResultsTable/>
            </BasicPageContainer>
        );
    }
}

const mapStateToProps = (state: State) => {
    return {
        gamesQuery: state.gamesQuery ? state.gamesQuery.query : null,
        gamesResults: state.gamesQuery ? state.gamesQuery.results : [],
        onSearchClicked: (query: GamesQuery) => Controller.queryGames(query)
    }
};

const DashboardPage = connect(mapStateToProps)(dashboardPage);
export default DashboardPage;