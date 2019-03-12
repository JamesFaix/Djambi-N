import * as React from 'react';
import * as Modal from 'react-modal';
import ActionButton from '../../controls/actionButton';
import { Player, PlayerStatus } from '../../../api/model';
import Dropdown from '../../controls/dropdown';
import * as Sprintf from 'sprintf-js';

export interface StatusChangeModalProps {
    onOk : (playerId : number) => void,
    onCancel : () => void,
    targetStatus : PlayerStatus,
    playerOptions : Player[]
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
                this.state = {
                    actingPlayer: props.playerOptions[0]
                };
                break;

            default:
                this.state = {
                    actingPlayer: null
                };
                break;
        }
    }

    render() {
       return (
            <Modal
                isOpen={true}
            >
                {this.renderMessage()}
                <br/>
                {this.renderPlayerDropdown()}
                <div>
                    {this.renderOkButton()}
                    <ActionButton
                        label="Cancel"
                        onClick={() => this.props.onCancel()}
                    />
                </div>
            </Modal>
        );
    }

    private renderMessage() {
        const p = this.state.actingPlayer;
        const status = this.props.targetStatus;

        if (status === PlayerStatus.AcceptsDraw) {
            if (p === null) {
                return <div>{Sprintf.sprintf("Select a player to accept a draw.")}</div>;
            } else {
                return (
                    <div>
                        <p>{Sprintf.sprintf("%s, are you sure you want to accept a draw?", p.name)}</p>
                        <p>
                            If all other remaining players also accept a draw, the game will end and
                            no one will win or lose. You can undo this as long as all other
                            remaining players have not already accepted a draw.
                        </p>
                    </div>
                );
            }
        } else if (status === PlayerStatus.Conceded) {
            if (p === null) {
                return <div>{Sprintf.sprintf("Select a player to concede.")}</div>;
            } else {
                return (
                    <div>
                        <p>{Sprintf.sprintf("%s, are you sure you want to concede?", p.name)}</p>
                        <p>
                            This cannot be undone. If it is not currently your turn,
                            this will take effect as soon as your next turn starts.
                            You will still be able to watch other players finish the game.
                        </p>
                    </div>
                );
            }
        } else {
            throw "Unsupported player status";
        }
    }

    private renderPlayerDropdown() {
        if (this.props.playerOptions.length < 2) {
            return undefined;
        }

        let items : [string, Player][] = [];
        items.push(["(Choose player)", null]);
        items = items.concat(
            this.props.playerOptions
            .map(p => {
                //Without the annotation, TypeScript interprets this
                //as an array of a union type rather than a single tuple type.
                let tup : [string, Player] = [p.name, p];
                return tup;
            }));

        const current = items
            .find(tup => tup[1].id === this.state.actingPlayer.id);

        return (
            <Dropdown
                name="ActingPlayer"
                currentValue={current[1]}
                onChange={(_, player) => this.setState({actingPlayer: player})}
                items={items}
            />
        );
    }

    private renderOkButton() {
        if (this.state.actingPlayer === null) {
            return undefined;
        }

        return (
            <ActionButton
                label="OK"
                onClick={() => this.props.onOk(this.state.actingPlayer.id)}
            />
        );
    }
}