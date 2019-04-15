import * as React from 'react';
import Icon, { IconKind } from './icon';
import { PlayerStatus, Player, PlayerKind } from '../../api/model';

interface IconDetails {
    hint : string,
    kind : IconKind,
    isActive : boolean
}

export interface PlayerStatusIconProps {
    player : Player
}

export default class PlayerStatusIcon extends React.Component<PlayerStatusIconProps> {

    private getDetails(player : Player) : IconDetails {
        switch (player.status) {
            case PlayerStatus.Pending:
                return {
                    hint: "Pending",
                    kind: IconKind.Pending,
                    isActive: false
                };
            case PlayerStatus.Alive:
                return {
                    hint: "Alive",
                    kind: IconKind.Alive,
                    isActive: player.kind !== PlayerKind.Neutral
                };
            case PlayerStatus.AcceptsDraw:
                return {
                     hint: "Will accept draw",
                    kind: IconKind.AcceptDraw,
                    isActive: player.kind !== PlayerKind.Neutral
                    };
            case PlayerStatus.Conceded:
                return {
                    hint: "Conceded",
                    kind: IconKind.Concede,
                    isActive: false
                };
            case PlayerStatus.WillConcede:
                return {
                    hint: "Will concede at the start of next turn",
                    kind: IconKind.Concede,
                    isActive: true
                };
            case PlayerStatus.Eliminated:
                return {
                    hint: "Eliminated",
                    kind: IconKind.Eliminated,
                    isActive: false
                 };
            case PlayerStatus.Victorious:
                return {
                     hint: "Victorious",
                     kind: IconKind.Victorious,
                     isActive: true
                     };
            default:
                throw "Invalid player status.";
        }
    }

    public render() : JSX.Element {
        const details = this.getDetails(this.props.player);
        const style = details.isActive
            ? undefined
            : {color: "silver"};

        return (
            <div style={style}>
                <Icon
                    kind={details.kind}
                    hint={details.hint}
                />
            </div>
        );
    }
}