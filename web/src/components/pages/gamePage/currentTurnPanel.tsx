import * as React from 'react';
import { Game, User, Player } from '../../../api/model';
import { Classes, Styles } from '../../../styles';
import Kernel from '../../../kernel';

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
        const color = Kernel.theme.getPlayerColor(currentPlayer.colorId);
        const style = Styles.combine([
            Styles.playerGlow(color),
            Styles.width(this.props.width),
            Styles.height(this.props.height)
        ]);
        return (
            <div className={Classes.thinBorder} style={style}>
                <div style={Styles.margin("10px")}>
                    {this.getPlayerNameHeader(currentPlayer)}
                    <br/>
                    <br/>
                    <div className={Classes.flex}>
                        <div style={Styles.width("45%")}>
                            {this.getSelectionsDescription()}
                        </div>
                        <div style={Styles.width("10%")}/>
                        <div style={Styles.width("45%")}>
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
            .map(s => <p>{Kernel.theme.getSelectionDescription(s, game)}</p>);

        return (
            <div style={this.props.textStyle}>
                Selections:
                <br/>
                <div className={Classes.indented}>
                    {descriptions}
                </div>
            </div>
        );
    }

    private getSelectionPrompt() {
        const prompt = Kernel.theme.getSelectionPrompt(
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
            <div style={Styles.grid()}>
                {
                    turn.requiredSelectionKind === null
                        ? <button
                            onClick={_ => this.props.commitTurn(gameId)}
                            style={Styles.width("70%")}
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
                            style={Styles.width("70%")}
                        >
                            Reset
                        </button>
                        : ""
                }
            </div>
        );
    }
}