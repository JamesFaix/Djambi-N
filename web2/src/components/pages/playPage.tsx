import * as React from 'react';
import { Game, User, Privilege } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import TimelineBar from '../timelineBar/timelineBar';
import LoadGameFull from '../utilities/loadGameFull';
import BoardSection from '../sections/boardSection';
import PlayPageContainer from '../sections/playPageContainer';
import * as StoreNavigation from '../../store/navigation';

interface PlayPageProps {
    game : Game,
    user : User
}

class playPage extends React.Component<PlayPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;
        const u = this.props.user;

        const navOptions : StoreNavigation.State = {
            enableDashboard: true,
            enableLobby: true,
            enableDiplomacy: true,
            enableSnapshots: u && u.privileges.includes(Privilege.Snapshots),
            gameId: gameId
        };

        return (
            <PlayPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfGameNotInProgress/>
                <SetNavigationOptions options={navOptions}/>
                <LoadGameFull gameId={gameId}/>
                <BoardSection/>
                <TimelineBar/>
            </PlayPageContainer>
        );
    }
}

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game,
        user: state.session.user
    };
}

const PlayPage = connect(mapStateToProps)(playPage);

export default PlayPage;