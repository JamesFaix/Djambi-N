import * as React from 'react';
import { AppState, NavigationState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { User, Game } from '../../api/model';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';

interface DiplomacyPageProps {
    user : User,
    game : Game
}

class diplomacyPage extends React.Component<DiplomacyPageProps>{
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
                Content
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        user: state.session.user,
        game: state.activeGame.game
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {

    };
}

const DiplomacyPage = connect(mapStateToProps, mapDispatchToProps)(diplomacyPage);
export default DiplomacyPage;