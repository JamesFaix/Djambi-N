import * as React from 'react';
import { NavigationState } from '../../store/state';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';
import DiplomacyPlayersTable from '../tables/diplomacyPlayersTable';

export default class DiplomacyPage extends React.Component<{}>{
    render() {
        const gameId = (this.props as any).match.params.gameId;

        const navOptions : NavigationState = {
            enableDashboard: true,
            enableLobby: true,
            gameId: gameId,
            enablePlay: true
        };

        return (
            <div style={Styles.pageContainer()}>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfGameNotInProgress/>
                <SetNavigationOptions options={navOptions}/>
                <DiplomacyPlayersTable/>
            </div>
        );
    }
}