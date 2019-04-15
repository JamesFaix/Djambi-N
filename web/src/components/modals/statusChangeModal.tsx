import * as React from 'react';
import * as Modal from 'react-modal';
import { Player, PlayerStatus } from '../../api/model';
import Dropdown, { DropdownItem } from '../controls/dropdown';
import * as Sprintf from 'sprintf-js';
import { Kernel as K } from '../../kernel';
import Button, { ButtonKind } from '../controls/button';

export interface StatusChangeModalProps {
    onOk : (playerId : number) => void,
    onCancel : () => void,
    targetStatus : PlayerStatus,
    playerOptions : Player[],
    setPlayer : (player : Player) => void
}

export interface StatusChangeModalState {
    actingPlayer : Player
}

export default class StatusChangeModal extends React.Component<StatusChangeModalProps, StatusChangeModalState> {
    constructor(props : StatusChangeModalProps) {
        super(props);

        switch (props.playerOptions.length) {
            case 0:
                throw "Cannot show status change modal if no player options.";

            case 1:
                const p = props.playerOptions[0];
                this.props.setPlayer(p);
                this.state = { actingPlayer: p };
                break;

            default:
                this.state = { actingPlayer: null };
                break;
        }
    }

    private onPlayerSelected(player : Player) {
        this.props.setPlayer(player);
        this.setState({actingPlayer: player});
    }

    //--- Rendering ---

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
                {this.renderHeader()}
                {this.renderBody()}
                <br/>
                <div className={K.classes.combine([K.classes.table, K.classes.centerAligned])}>
                    {this.renderPlayerDropdown()}
                    <br/>
                    <br/>
                    <div>
                        {this.renderOkButton()}
                        <Button
                            kind={ButtonKind.Action}
                            label="No, go back to the game"
                            onClick={() => this.props.onCancel()}
                        />
                    </div>
                </div>
            </Modal>
        );
    }

    private renderHeader() {
        const p = this.state.actingPlayer;

        let text : string;

        if (p === null) {
            switch (this.props.targetStatus) {
                case PlayerStatus.AcceptsDraw:
                    text = "Select a player to accept a draw.";
                    break;
                case PlayerStatus.Alive:
                    text = "Select a player to decline a draw.";
                    break;
                case PlayerStatus.Conceded:
                    text = "Select a player to concede";
                    break;
                default:
                    throw "Unsupported player status";
            }
        } else {
            switch (this.props.targetStatus) {
                case PlayerStatus.AcceptsDraw:
                    text = Sprintf.sprintf("%s, are you sure you want to accept a draw?", p.name);
                    break;
                case PlayerStatus.Alive:
                    text = Sprintf.sprintf("%s, are you sure you want to decline a draw?", p.name);
                    break;
                case PlayerStatus.Conceded:
                    text = Sprintf.sprintf("%s, are you sure you want to concede?", p.name);
                    break;
                default:
                    throw "Unsupported player status";
            }
        }

        return (
            <div
                className={K.classes.centerAligned}
                style={K.styles.bold()}
            >
                {text}
            </div>
        );
    }

    private renderBody() {
        const p = this.state.actingPlayer;
        if (p === null) {
            return undefined;
        }

        switch (this.props.targetStatus) {
            case PlayerStatus.AcceptsDraw:
                return (
                    <div>
                        <p>
                            If all other remaining players also accept a draw, the game will end and
                            no one will win or lose.
                        </p>
                        <p>
                            You can undo this as long as all other
                            remaining players have not already accepted a draw.
                        </p>
                    </div>
                );

            case PlayerStatus.Alive:
                return (
                    <div>
                        <p>
                            You can accept a draw again later, if you change your mind.
                        </p>
                    </div>
                );

            case PlayerStatus.Conceded:
                return (
                    <div>
                        <p>
                            This cannot be undone.
                        </p>
                        <p>
                            If it is not currently your turn,
                            this will take effect as soon as your next turn starts.
                        </p>
                        <p>
                            You will still be able to watch other players finish the game.
                        </p>
                    </div>
                );

            default:
                throw "Unsupported player status";
        }
    }

    private renderPlayerDropdown() {
        if (this.props.playerOptions.length < 2) {
            return undefined;
        }

        let items : DropdownItem<Player>[] = [];

        items.push({
            label: "(Choose player)",
            value: null
        });

        items = items.concat(
            this.props.playerOptions
                .map(p => {
                    const result = {
                        label: p.name,
                        value: p
                    };
                    return result;
                }));

        let currentPlayer = null;

        if (this.state.actingPlayer) {
            const item = items
                .find(x => x.value !== null
                    && x.value.id === this.state.actingPlayer.id);
            if (item) {
                currentPlayer = item.value;
            }
        }

        return (
            <Dropdown
                name="ActingPlayer"
                currentValue={currentPlayer}
                onChange={(_, player) => this.onPlayerSelected(player)}
                items={items}
            />
        );
    }

    private renderOkButton() {
        if (this.state.actingPlayer === null) {
            return undefined;
        }

        return (
            <Button
                kind={ButtonKind.Action}
                label="Yes, I'm sure"
                onClick={() => this.props.onOk(this.state.actingPlayer.id)}
            />
        );
    }
}