import * as React from 'react';
import { Dispatch } from 'redux';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Game, User, TurnStatus } from '../../api/model';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import GameStoreFlows from '../../storeFlows/game';

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
                icon={Icons.PlayerActions.resetTurn}
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
                icon={Icons.PlayerActions.endTurn}
                onClick={() => this.props.endTurn(this.props.game.id)}
            />
        );
    }
}

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user,
        game: state.activeGame.game
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        endTurn: (gameId : number) => GameStoreFlows.endTurn(gameId)(dispatch),
        resetTurn: (gameId : number) => GameStoreFlows.resetTurn(gameId)(dispatch)
    };
}

const CurrentTurnActionsBar = connect(mapStateToProps, mapDispatchToProps)(currentTurnActionsBar);
export default CurrentTurnActionsBar;