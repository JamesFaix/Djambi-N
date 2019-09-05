import * as React from 'react';
import { Player, Game, PlayerKind } from '../../api/model';
import * as LobbySeats from '../../viewModel/lobbySeats';
import IconBox from './iconBox';
import { Icons } from '../../utilities/icons';
import ThemeService from '../../themes/themeService';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Theme } from '../../themes/model';

interface PlayerNoteIconProps {
    player : Player,
    game : Game,
    theme : Theme
}

class playerNoteIcon extends React.Component<PlayerNoteIconProps> {
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
            ? ThemeService.getPlayerColor(this.props.theme, hostPlayer.colorId)
            : "black";
        const note = LobbySeats.getPlayerNote(this.props.player, this.props.game);

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
}

const mapStateToProps = (state : State) => {
    return {
        theme: state.display.theme
    };
}

const PlayerNoteIcon = connect(mapStateToProps)(playerNoteIcon);
export default PlayerNoteIcon;