import * as React from 'react';
import { useSelector } from 'react-redux';
import GamesSearchResultsTable from '../pageSections/gamesSearchResultsTable';
import { State as AppState } from '../../store/root';
import GamesSearchForm from '../pageSections/gamesSearchForm';
import BasicPageContainer from '../containers/basicPageContainer';
import Controller from '../../controllers/controller';
import { SectionHeader } from '../controls/headers';
import { List } from '../../utilities/collections';

const GamesSearchPage : React.SFC<{}> = _ => {
    const games = useSelector((state : AppState) => {
        const results = state.search.results;
        return List.sortBy(results, g => g.id, true);
    });
    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn();
    });
    return (
        <BasicPageContainer>
            <GamesSearchForm/>
            <br/>
            <br/>
            <SectionHeader text="Results"/>
            <GamesSearchResultsTable games={games}/>
        </BasicPageContainer>
    );
}
export default GamesSearchPage;