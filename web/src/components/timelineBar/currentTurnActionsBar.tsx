import * as React from 'react';
import { Dispatch } from 'redux';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Game, User, TurnStatus } from '../../api/model';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import GameStoreFlows from '../../storeFlows/game';
import { Theme } from '../../themes/model';

interface CurrentTurnActionsBarProps {
    user : User,
    game : Game,
    endTurn : (gameId : number) => void,
    resetTurn : (gameId : number) => void,
    theme : Theme
}

class currentTurnActionsBar extends React.Component<CurrentTurnActionsBarProps> {
    render() {
        return (
            <div style={{
                display: "flex"
            }}>
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
        const disabled = !this.isCurrentUsersTurn() ||
            this.props.game.currentTurn.selections.length === 0;

        return (
            <IconButton
                icon={Icons.PlayerActions.resetTurn}
                onClick={() => this.props.resetTurn(this.props.game.id)}
                style={{
                    backgroundColor: disabled ? null : this.props.theme.colors.negativeButtonBackground,
                    flex: 1
                }}
                disabled={disabled}
            />
        );
    }

    private renderEndButton() {
        const disabled = !this.isCurrentUsersTurn() ||
            this.props.game.currentTurn.status !== TurnStatus.AwaitingCommit;

        return (
            <IconButton
                icon={Icons.PlayerActions.endTurn}
                onClick={() => this.props.endTurn(this.props.game.id)}
                style={{
                    backgroundColor: disabled ? null : this.props.theme.colors.positiveButtonBackground,
                    flex: 1
                }}
                disabled={disabled}
            />
        );
    }
}

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user,
        game: state.activeGame.game,
        theme: state.display.theme
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