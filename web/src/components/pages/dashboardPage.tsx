import * as React from 'react';
import { connect } from 'react-redux';
import { GamesQuery, Game, User } from '../../api/model';
import GamesSearchResultsTable from '../tables/gamesSearchResultsTable';
import { State } from '../../store/root';
import GamesSearchForm from '../forms/gamesSearchForm';
import BasicPageContainer from '../sections/basicPageContainer';
import Controller from '../../controller';
import ControllerEffects from '../../controllerEffects';

const dashboardPage : React.SFC<{
    user : User,
    gamesQuery : GamesQuery,
    gamesResults : Game[],
    onSearchClicked : (query: GamesQuery) => void
}> = props => {
    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);

    return (
        <BasicPageContainer>
            <GamesSearchForm/>
            <br/>
            <br/>
            <GamesSearchResultsTable/>
        </BasicPageContainer>
    );
}

const mapStateToProps = (state: State) => {
    return {
        user : state.session.user,
        gamesQuery: state.gamesQuery ? state.gamesQuery.query : null,
        gamesResults: state.gamesQuery ? state.gamesQuery.results : [],
        onSearchClicked: (query: GamesQuery) => Controller.queryGames(query)
    }
};

const DashboardPage = connect(mapStateToProps)(dashboardPage);
export default DashboardPage;