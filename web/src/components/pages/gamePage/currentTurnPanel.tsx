import * as React from 'react';
import { Game, Player, User } from '../../../api/model';
import { Kernel as K } from '../../../kernel';

export interface CurrentTurnPanelProps {
    game : Game,
    user : User,
    commitTurn : (gameId : number) => void,
    resetTurn : (gameId : number) => void
    height : string,
    width : string,
    textStyle : React.CSSProperties
}

export default class CurrentTurnPanel extends React.Component<CurrentTurnPanelProps> {

    render() {
        if (this.props.game.currentTurn === null){
            return <div></div>;
        }

        const currentPlayer = this.getCurrentPlayer(this.props.game);
        const color = K.theme.getPlayerColor(currentPlayer.colorId);
        const style = K.styles.combine([
            K.styles.playerGlow(color),
            K.styles.width(this.props.width),
            K.styles.height(this.props.height)
        ]);
        return (
            <div className={K.classes.thinBorder} style={style}>
                <div style={K.styles.margin("10px")}>
                    {this.getPlayerNameHeader(currentPlayer)}
                    <br/>
                    <br/>
                    <div className={K.classes.flex}>
                        <div style={K.styles.width("45%")}>
                            {this.getSelectionsDescription()}
                        </div>
                        <div style={K.styles.width("10%")}/>
                        <div style={K.styles.width("45%")}>
                            {this.getSelectionPrompt()}
                            {this.renderActionButtons()}
                        </div>
                    </div>
                </div>
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

    private getPlayerNameHeader(player : Player) {
        return player.name + "'s turn";
    }

    private getSelectionsDescription() {
        const game = this.props.game;

        const descriptions = game.currentTurn.selections
            .map((s, i) => <p key={"row" + i}>{K.copy.getSelectionDescription(s, game)}</p>);

        return (
            <div style={this.props.textStyle}>
                Selections:
                <br/>
                <div className={K.classes.indented}>
                    {descriptions}
                </div>
            </div>
        );
    }

    private getSelectionPrompt() {
        const prompt = K.theme.getSelectionPrompt(
            this.props.game.currentTurn.requiredSelectionKind);

        return prompt;
    }

    private renderActionButtons() {
        if (!this.isCurrentPlayerSelfOrGuest()) {
            return "";
        }

        const gameId = this.props.game.id;
        const turn = this.props.game.currentTurn;

        return (
            <div style={K.styles.grid()}>
                {
                    turn.requiredSelectionKind === null
                        ? <button
                            onClick={_ => this.props.commitTurn(gameId)}
                            style={K.styles.width("70%")}
                        >
                            Commit
                        </button>
                        : ""
                }
                <br/>
                {
                    turn.selections.length > 0
                        ? <button
                            onClick={_ => this.props.resetTurn(gameId)}
                            style={K.styles.width("70%")}
                        >
                            Reset
                        </button>
                        : ""
                }
            </div>
        );
    }
}