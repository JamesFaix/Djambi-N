import * as React from 'react';
import { useSelector } from 'react-redux';
import GamesSearchResultsTable from '../pageSections/gamesSearchResultsTable';
import { State as AppState } from '../../store/root';
import GamesSearchForm from '../pageSections/gamesSearchForm';
import BasicPageContainer from '../containers/basicPageContainer';
import Controller from '../../controllers/controller';
import { SectionHeader } from '../controls/headers';

const GamesSearchPage : React.SFC<{}> = _ => {
    const games = useSelector((state : AppState) => state.search.results);
    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn();
    });
    return (
        <BasicPageContainer>
            <SectionHeader text="Search games"/>
            <GamesSearchForm/>
            <br/>
            <br/>
            <SectionHeader text="Results"/>
            <GamesSearchResultsTable games={games}/>
        </BasicPageContainer>
    );
}
export default GamesSearchPage;