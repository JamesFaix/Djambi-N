import * as React from 'react';
import '../index.css';
import { Game, User } from '../api/model';
import ThemeService from '../themes/themeService';
import ActionButton from './controls/actionButton';

export interface CurrentTurnPanelProps {
    game : Game,
    theme : ThemeService,
    user : User,
    commitTurn : (gameId : number) => void,
    resetTurn : (gameId : number) => void
}

export default class CurrentTurnPanel extends React.Component<CurrentTurnPanelProps> {

    render() {
        const player = this.getCurrentPlayer(this.props.game);

        return (
            <div className="thinBorder">
                {player.name}
                <br/>
                {this.getSelectionsDescription()}
                <br/>
                {this.getSelectionPrompt()}
                <br/>
                {this.renderActionButtons()}
            </div>
        );
    }

    private isCurrentPlayerSelfOrGuest() {
        const currentPlayer = this.getCurrentPlayer(this.props.game);
        return currentPlayer.userId === this.props.user.id;
    }

    private getCurrentPlayer(game : Game) {
        return game.players
            .find(p => p.id === game.turnCycle[0]);
    }

    private getSelectionsDescription() {
        const game = this.props.game;
        const turn = game.currentTurn;

        if (turn === null) {
            return "";
        }

        return turn.selections
            .map(s => this.props.theme.getSelectionDescription(s, game))
            .join("");
    }

    private getSelectionPrompt() {
        const turn = this.props.game.currentTurn;

        if (turn === null) {
            return "";
        }

        return this.props.theme.getSelectionPrompt(turn.requiredSelectionKind);
    }

    private renderActionButtons() {
        const turn = this.props.game.currentTurn;
        if (!this.isCurrentPlayerSelfOrGuest() || turn === null) {
            return "";
        }

        const gameId = this.props.game.id;
        return (
            <div>
                {
                    turn.requiredSelectionKind === null
                        ? <ActionButton label="Commit" onClick={() => this.props.commitTurn(gameId)}/>
                        : ""
                }
                {
                    turn.selections.length > 0
                        ? <ActionButton label="Reset" onClick={() => this.props.resetTurn(gameId)}/>
                        : ""
                }
            </div>
        );
    }
}