import * as React from 'react';
import { Game, Player, User, Turn, TurnStatus } from '../../api/model';
import { Kernel as K } from '../../kernel';
import * as Sprintf from 'sprintf-js';

export interface CurrentTurnPanelProps {
    game : Game,
    user : User,
    height : string,
    width : string,
    textStyle : React.CSSProperties
}

export default class CurrentTurnPanel extends React.Component<CurrentTurnPanelProps> {

    public render() : JSX.Element {
        const turn = this.props.game.currentTurn;
        if(turn === null) {
            return undefined;
        }

        const player = this.getCurrentPlayer(this.props.game);
        const color = K.theme.getPlayerColor(player.colorId);
        const style = K.styles.combine([
            K.styles.playerGlow(color),
            K.styles.width(this.props.width),
            K.styles.height(this.props.height)
        ]);

        return (
            <div className={K.classes.thinBorder} style={style}>
                <div style={K.styles.margin("10px")}>
                    {
                        player.userId === this.props.user.id
                            ? this.renderForCurrentPlayer(player, turn)
                            : this.renderForOtherPlayer(player)
                    }
                </div>
            </div>
        );
    }

    private getCurrentPlayer(game : Game) {
        return game.players
            .find(p => p.id === game.turnCycle[0]);
    }

    private renderForOtherPlayer(player : Player) {
        return (
            <div>
                {`Waiting on ${player.name}...`}
            </div>
        );
    }

    private renderForCurrentPlayer(player : Player, turn : Turn) {
        return (
            <div>
                <p>
                    {Sprintf.sprintf(K.theme.getTurnPrompt(turn), player.name)}
                </p>
                <div>
                    {this.getSelectionsDescription(turn)}
                </div>
            </div>
        );
    }

    private getSelectionsDescription(turn : Turn) {
        const game = this.props.game;

        const descriptions = turn.selections
            .map((s, i) => <p key={"row" + i}>{K.copy.getSelectionDescription(s, game)}</p>);

        const style = K.styles.combine([this.props.textStyle, {fontStyle: "italic"}]);

        return (
            <div style={style}>
                Pending turn:
                <br/>
                <div className={K.classes.indented}>
                    {descriptions}
                    {this.renderEllipsisIfSelectionRequired(turn)}
                </div>
            </div>
        );
    }

    private renderEllipsisIfSelectionRequired(turn : Turn) {
        return turn.status === TurnStatus.AwaitingSelection
            ? <p>...</p>
            : undefined;
    }
}