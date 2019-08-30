import { PlayerStatus, PlayerKind, Player } from "../../api/model";
import * as React from 'react';
import IconBox from "./iconBox";
import { Icons } from "../../utilities/icons";

interface PlayerStatusIconProps {
    player : Player
}

const PlayerStatusIcon : React.SFC<PlayerStatusIconProps> = props => {
    const p = props.player;
    const s = p.status;

    const isActive = p.kind !== PlayerKind.Neutral &&
        (s === PlayerStatus.AcceptsDraw ||
        s === PlayerStatus.Alive ||
        s === PlayerStatus.Victorious);

    const color = isActive ? null : "gray";

    return (
        <IconBox
            icon={Icons.playerStatus(s)}
            color={color}
        />
    );
}

export default PlayerStatusIcon;