import { PlayerStatus, PlayerKind, Player } from "../../api/model";
import { IconDefinition } from "@fortawesome/free-solid-svg-icons";
import * as React from 'react';
import Icons from "../../utilities/icons";
import IconBox from "./iconBox";

interface PlayerStatusIconProps {
    player : Player
}

interface PlayerIconDetails {
    hint : string,
    isActive : boolean,
    icon : IconDefinition
}

export default class PlayerStatusIcon extends React.Component<PlayerStatusIconProps> {
    render() {
        const details = this.getIconDetails(this.props.player);
        const color = details.isActive ? null : "gray";
        return (
            <IconBox
                icon={details.icon}
                title={details.hint}
                color={color}
            />
        );
    }

    private getIconDetails(player : Player) : PlayerIconDetails {
        const i = Icons.PlayerStatus;

        switch (player.status) {
            case PlayerStatus.Pending:
                return {
                    hint: "Pending",
                    icon: i.Pending,
                    isActive: false
                };
            case PlayerStatus.Alive:
                return {
                    hint: "Alive",
                    icon: i.Alive,
                    isActive: player.kind !== PlayerKind.Neutral
                };
            case PlayerStatus.AcceptsDraw:
                return {
                    hint: "Will accept draw",
                    icon: i.AcceptsDraw,
                    isActive: player.kind !== PlayerKind.Neutral
                };
            case PlayerStatus.Conceded:
                return {
                    hint: "Conceded",
                    icon: i.Conceded,
                    isActive: false
                };
            case PlayerStatus.WillConcede:
                return {
                    hint: "Will concede at the start of next turn",
                    icon: i.Conceded,
                    isActive: true
                };
            case PlayerStatus.Eliminated:
                return {
                    hint: "Eliminated",
                    icon: i.Eliminated,
                    isActive: false
                };
            case PlayerStatus.Victorious:
                return {
                    hint: "Victorious",
                    icon: i.Victorious,
                    isActive: true
                };
            default:
                throw "Invalid player status.";
        }
    }
}