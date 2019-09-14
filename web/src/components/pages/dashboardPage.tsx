import * as React from 'react';
import { useSelector } from 'react-redux';
import GamesSearchResultsTable from '../pageSections/gamesSearchResultsTable';
import { State as AppState } from '../../store/root';
import GamesSearchForm from '../pageSections/gamesSearchForm';
import BasicPageContainer from '../containers/basicPageContainer';
import Controller from '../../controllers/controller';

const DashboardPage : React.SFC<{}> = _ => {
    const games = useSelector((state : AppState) => state.gamesQuery.results);
    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn();
    });
    return (
        <BasicPageContainer>
            <GamesSearchForm/>
            <br/>
            <br/>
            <GamesSearchResultsTable games={games}/>
        </BasicPageContainer>
    );
}
export default DashboardPage;