import * as React from 'react';
import { Player, Game, PlayerKind } from '../../api/model';
import Colors from '../../utilities/colors';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import * as LobbySeats from '../../viewModel/lobbySeats';
import Icons from '../../utilities/icons';

interface PlayerNoteIconProps {
    player : Player,
    game : Game
}

export default class PlayerNoteIcon extends React.Component<PlayerNoteIconProps> {
    render() {
        if (!this.props.player){
            return null;
        }

        const isGuest = this.props.player.kind === PlayerKind.Guest;
        if (!isGuest) {
            return null;
        }

        const hostPlayer = this.props.game.players
            .find(p => p.userId === this.props.player.userId
                && p.kind === PlayerKind.User);

        const color = hostPlayer.colorId
                ? Colors.getColorFromPlayerColorId(hostPlayer.colorId)
                : "black";
        const note = LobbySeats.getPlayerNote(this.props.player, this.props.game);

        return (
            <FontAwesomeIcon
                icon={Icons.guest}
                style={{color: color}}
                title={note}
            />
        );
    }
}