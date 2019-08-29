import { PlayerStatus, PlayerKind, Player } from "../../api/model";
import { IconDefinition } from "@fortawesome/free-solid-svg-icons";
import * as React from 'react';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Icons from "../../utilities/icons";

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
        const style = {
            color: details.isActive ? "black" : "gray"
        };
        return (
            <FontAwesomeIcon
                icon={details.icon}
                style={style}
                title={details.hint}
            />
        );
    }

    private getIconDetails(player : Player) : PlayerIconDetails {
        switch (player.status) {
            case PlayerStatus.Pending:
                return {
                    hint: "Pending",
                    icon: Icons.playerStatusPending,
                    isActive: false
                };
            case PlayerStatus.Alive:
                return {
                    hint: "Alive",
                    icon: Icons.playerStatusAlive,
                    isActive: player.kind !== PlayerKind.Neutral
                };
            case PlayerStatus.AcceptsDraw:
                return {
                    hint: "Will accept draw",
                    icon: Icons.playerStatusAcceptsDraw,
                    isActive: player.kind !== PlayerKind.Neutral
                };
            case PlayerStatus.Conceded:
                return {
                    hint: "Conceded",
                    icon: Icons.playerStatusConceded,
                    isActive: false
                };
            case PlayerStatus.WillConcede:
                return {
                    hint: "Will concede at the start of next turn",
                    icon: Icons.playerStatusConceded,
                    isActive: true
                };
            case PlayerStatus.Eliminated:
                return {
                    hint: "Eliminated",
                    icon: Icons.playerStatusEliminated,
                    isActive: false
                };
            case PlayerStatus.Victorious:
                return {
                    hint: "Victorious",
                    icon: Icons.playerStatusVictorious,
                    isActive: true
                };
            default:
                throw "Invalid player status.";
        }
    }
}