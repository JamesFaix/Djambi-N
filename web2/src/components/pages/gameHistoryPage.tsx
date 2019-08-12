import * as React from 'react';
import { Game, GameStatus, Event } from '../../api/model';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { AppState } from '../../store/state';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import Styles from '../../styles/styles';
import LoadGameAndHistory from '../utilities/loadGameAndHistory';
import GameHistorySection from '../gameHistory/gameHistorySection';

interface GameHistoryPageProps {
    game : Game,
    history : Event[]
}

class gameHistoryPage extends React.Component<GameHistoryPageProps> {
    render() {
        const gameId = (this.props as any).match.params.gameId;

        const navOptions = {
            enableDashboard: true,
            enableLobby: true,
            enablePlay: this.props.game && this.props.game.status === GameStatus.InProgress,
            gameId: gameId
        };

        return (
            <div style={Styles.pageContainer()}>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={navOptions}/>
                <LoadGameAndHistory gameId={gameId}/>
                <GameHistorySection/>
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        game: state.activeGame.game,
        history: state.activeGame.history
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {

    };
}

const GameHistoryPage = connect(mapStateToProps, mapDispatchToProps)(gameHistoryPage);

export default GameHistoryPage;