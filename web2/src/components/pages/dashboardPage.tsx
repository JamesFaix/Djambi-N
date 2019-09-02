import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { GamesQuery, Game } from '../../api/model';
import GamesSearchResultsTable from '../tables/gamesSearchResultsTable';
import { State } from '../../store/root';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import GamesSearchForm from '../forms/gamesSearchForm';
import ApiActions from '../../apiActions';
import BasicPageContainer from '../sections/basicPageContainer';

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

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onSearchClicked: (query: GamesQuery) => ApiActions.queryGames(query)(dispatch)
    };
};

const DashboardPage = connect(mapStateToProps, mapDispatchToProps)(dashboardPage);

export default DashboardPage;