import * as React from 'react';
import { useSelector } from 'react-redux';
import GamesSearchResultsTable from '../tables/gamesSearchResultsTable';
import { State as AppState } from '../../store/root';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import GamesSearchForm from '../forms/gamesSearchForm';
import BasicPageContainer from '../containers/basicPageContainer';

const DashboardPage : React.SFC<{}> = _ => {
    const games = useSelector((state : AppState) => state.gamesQuery.results);
    return (
        <BasicPageContainer>
            <RedirectToLoginIfNotLoggedIn/>
            <GamesSearchForm/>
            <br/>
            <br/>
            <GamesSearchResultsTable games={games}/>
        </BasicPageContainer>
    );
}
export default DashboardPage;