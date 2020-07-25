import * as React from 'react';
import { useSelector } from 'react-redux';
import GamesSearchResultsTable from '../pageSections/gamesSearchResultsTable';
import { State as AppState } from '../../store/root';
import BasicPageContainer from '../containers/basicPageContainer';
import Controller from '../../controllers/controller';
import { SectionHeader } from '../controls/headers';
import { sortedUniqBy, iteratee } from 'lodash';

const DashboardPage : React.SFC<{}> = _ => {
    const games = useSelector((state : AppState) => {
        const results = state.search.recent;
        return sortedUniqBy(results, iteratee('id')).reverse();
    });
    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn();
    });
    return (
        <BasicPageContainer>
            <SectionHeader text="Recent games"/>
            <GamesSearchResultsTable games={games}/>
        </BasicPageContainer>
    );
}
export default DashboardPage;