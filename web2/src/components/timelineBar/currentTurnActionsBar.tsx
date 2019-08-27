import * as React from 'react';
import { Dispatch } from 'redux';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import { Game, User, TurnStatus } from '../../api/model';
import { faHandshake, faRecycle, faCheck } from '@fortawesome/free-solid-svg-icons';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import IconButton from '../controls/iconButton';
import ApiActions from '../../apiActions';

interface CurrentTurnActionsBarProps {
    user : User,
    game : Game,
    endTurn : (gameId : number) => void,
    resetTurn : (gameId : number) => void
}

class currentTurnActionsBar extends React.Component<CurrentTurnActionsBarProps> {
    render() {
        return (
            <div>
                {this.renderEndButton()}
                {this.renderResetButton()}
                {this.renderDiplomacyButton()}
            </div>
        );
    }

    private isCurrentUsersTurn() : boolean {
        const userId = this.props.user.id;
        const playerIds = this.props.game.players
            .filter(p => p.userId === userId)
            .map(p => p.id);
        const currentPlayerId = this.props.game.turnCycle[0];
        return playerIds.includes(currentPlayerId);
    }

    private renderResetButton(){
        if (!this.isCurrentUsersTurn() ||
            this.props.game.currentTurn.selections.length === 0
        ) {
            return null;
        }

        return (
            <IconButton
                title="Reset turn"
                icon={faRecycle}
                onClick={() => this.props.resetTurn(this.props.game.id)}
            />
        );
    }

    private renderEndButton() {
        if (!this.isCurrentUsersTurn() ||
            this.props.game.currentTurn.status !== TurnStatus.AwaitingCommit
        ) {
            return null;
        }

        return (
            <IconButton
                title={"Finish turn"}
                icon={faCheck}
                onClick={() => this.props.endTurn(this.props.game.id)}
            />
        );
    }

    private renderDiplomacyButton() {
        return (
            <IconButton
                title={"Concede or Draw"}
                icon={faHandshake}
                onClick={() => navigateTo(Routes.diplomacy(this.props.game.id))}
            />
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
        endTurn: (gameId : number) => ApiActions.endTurn(gameId)(dispatch),
        resetTurn: (gameId : number) => ApiActions.resetTurn(gameId)(dispatch)
    };
}

const CurrentTurnActionsBar = connect(mapStateToProps, mapDispatchToProps)(currentTurnActionsBar);
export default CurrentTurnActionsBar;