import * as React from 'react';
import * as Modal from 'react-modal';
import { Kernel as K } from '../../kernel';
import Button, { ButtonKind } from '../controls/button';
import { Game, PlayerStatus } from '../../api/model';
import { HomePageButton, MyGamesPageButton, FindGamesPageButton } from '../controls/navigationButtons';
import PlayersPanelTable from '../tables/playersPanelTable';
import { IconKind } from '../icons/icon';
import * as Sprintf from 'sprintf-js';

export interface GameOverModalProps {
    game : Game,
    closeWindow : () => void
}

export default class GameOverModal extends React.Component<GameOverModalProps> {

    public render() : JSX.Element {
        const style = {
            content : {
            top: "30%",
            bottom: "30%",
            left: "30%",
            right: "30%"
            }
        };

        return (
            <Modal
                isOpen={true}
                style={style}
            >
                <div className={K.classes.centerAligned}>
                    <div style={K.styles.bold()}>
                        Game over
                    </div>
                    <br/>
                    {this.getBodyText()}
                    <br/>
                    <br/>
                    <div style={{width: "200px", margin: "0 auto"}}>
                        <PlayersPanelTable
                            game={this.props.game}
                        />
                    </div>
                    <br/>
                    <div>
                        <HomePageButton/>
                        <MyGamesPageButton/>
                        <FindGamesPageButton/>
                        <Button
                            kind={ButtonKind.Action}
                            icon={IconKind.Close}
                            onClick={() => this.props.closeWindow()}
                            hint="Close window"
                        />
                    </div>
                </div>
            </Modal>
        );
    }

    private getBodyText() : string {
        const players = this.props.game.players;

        const winners = players.filter(p => p.status === PlayerStatus.Victorious);
        if (winners.length === 1) {
            return Sprintf.sprintf("%s won!", winners[0].name);
        }

        const drawers = players.filter(p => p.status === PlayerStatus.AcceptsDraw);
        if (drawers.length > 0) {
            const last = drawers.pop();
            let list = drawers.map(p => p.name).join(", ");
            list = list + " and " + last.name;
            return Sprintf.sprintf("%s accepted a draw.", list);
        }

        return "Everyone lost. It can only go up from here!";
    }
}