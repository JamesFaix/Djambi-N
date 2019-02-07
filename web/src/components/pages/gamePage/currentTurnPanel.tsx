import * as React from 'react';
import { Game, User, Player } from '../../../api/model';
import ThemeService from '../../../themes/themeService';
import ActionButton from '../../controls/actionButton';
import { Classes, Styles } from '../../../styles';

export interface CurrentTurnPanelProps {
    game : Game,
    theme : ThemeService,
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
        const color = this.props.theme.getPlayerColor(currentPlayer.colorId);
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
                    {this.getSelectionsDescription()}
                    {this.getSelectionPrompt()}
                    <br/>
                    {this.renderActionButtons()}
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
            .map(s => <p>{this.props.theme.getSelectionDescription(s, game)}</p>);

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
        const prompt = this.props.theme.getSelectionPrompt(
            this.props.game.currentTurn.requiredSelectionKind);

        return (
            <div>
                Required action:
                <br/>
                <div className={Classes.indented}>
                    {prompt}
                </div>
            </div>
        )
    }

    private renderActionButtons() {
        if (!this.isCurrentPlayerSelfOrGuest()) {
            return "";
        }

        const gameId = this.props.game.id;
        const turn = this.props.game.currentTurn;

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