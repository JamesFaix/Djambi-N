import * as React from 'react';
import { Player, Game, PlayerKind } from '../../api/model';
import * as LobbySeats from '../../viewModel/lobbySeats';
import IconBox from './iconBox';
import { Icons } from '../../utilities/icons';
import ThemeService from '../../themes/themeService';
import Selectors from '../../selectors';

const PlayerNoteIcon : React.SFC<{
    player : Player,
    game : Game
}> = props => {
    if (!props.player){
        return null;
    }

    const isGuest = props.player.kind === PlayerKind.Guest;
    if (!isGuest) {
        return null;
    }

    const hostPlayer = props.game.players
        .find(p => p.userId === props.player.userId
            && p.kind === PlayerKind.User);

    const theme = Selectors.theme();

    const color = hostPlayer.colorId
        ? ThemeService.getPlayerColor(theme, hostPlayer.colorId)
        : theme.colors.text;

    const note = LobbySeats.getPlayerNote(props.player, props.game);

    const info = {
        title: note,
        icon: Icons.PlayerNotes.guest
    };

    return (
        <IconBox
            icon={info}
            color={color}
        />
    );
}

export default PlayerNoteIcon;