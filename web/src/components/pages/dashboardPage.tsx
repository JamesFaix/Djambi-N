import * as React from 'react';
import { Dispatch } from 'redux';
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

const mapDispatchToProps = (_ : Dispatch) => {
    return {
        onSearchClicked: (query: GamesQuery) => Controller.queryGames(query)
    };
};

const DashboardPage = connect(mapStateToProps, mapDispatchToProps)(dashboardPage);

export default DashboardPage;