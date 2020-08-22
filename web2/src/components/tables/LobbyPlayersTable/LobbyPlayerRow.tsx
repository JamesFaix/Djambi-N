import React, { FC, useState } from 'react';
import {
  TableCell, TableRow, TextField, Button,
} from '@material-ui/core';
import {
  Add as AddIcon,
  Remove as RemoveIcon,
} from '@material-ui/icons';
import { useSelector } from 'react-redux';
import { GameDto, PlayerKind } from '../../../api-client';
import { LobbyPlayerViewModel, LobbyPlayerActionType } from './viewModel';
import { addPlayer, removePlayer } from '../../../controllers/gameController';
import { selectSession } from '../../../hooks/selectors';
import { useFormStyles } from '../../../styles/styles';
import { theme } from '../../../styles/materialTheme';

interface Props {
  player: LobbyPlayerViewModel,
  game: GameDto
}

const getActionIcon = (action: LobbyPlayerActionType | null) => {
  switch (action) {
    case LobbyPlayerActionType.AddGuest:
    case LobbyPlayerActionType.SelfJoin:
      return <AddIcon style={{ paddingRight: '5px' }} />;
    case LobbyPlayerActionType.Remove:
    case LobbyPlayerActionType.SelfQuit:
      return <RemoveIcon style={{ paddingRight: '5px' }} />;
    default:
      return <></>;
  }
};

const getActionLabel = (action: LobbyPlayerActionType | null) => {
  switch (action) {
    case LobbyPlayerActionType.AddGuest:
      return 'Add guest';
    case LobbyPlayerActionType.SelfJoin:
      return 'Join';
    case LobbyPlayerActionType.SelfQuit:
      return 'Quit';
    case LobbyPlayerActionType.Remove:
      return 'Remove';
    default:
      return '';
  }
};

const LobbyPlayerRow: FC<Props> = ({ player, game }) => {
  const styles = useFormStyles(theme);

  const [guestName, setGuestName] = useState('');
  const { user } = useSelector(selectSession);

  if (user === null) {
    return <></>;
  }

  let helperText = '';

  const isDuplicateName = game.players.find((p) => p.name === guestName);
  if (isDuplicateName) {
    helperText = `There is already a player named ${guestName}`;
  }

  const isValidName = !isDuplicateName;

  const addGuest = () => {
    addPlayer(game.id, {
      userId: user.id,
      name: guestName,
      kind: PlayerKind.Guest,
    });
  };

  const selfJoin = () => {
    addPlayer(game.id, {
      userId: user.id,
      name: user.name,
      kind: PlayerKind.User,
    });
  };

  const remove = () => {
    if (player.id === null) {
      throw new Error('Cannot remove player without ID.');
    }
    removePlayer(game.id, player.id);
  };

  const onClick = () => {
    switch (player.actionType) {
      case LobbyPlayerActionType.AddGuest:
        addGuest();
        break;
      case LobbyPlayerActionType.SelfJoin:
        selfJoin();
        break;
      case LobbyPlayerActionType.SelfQuit:
      case LobbyPlayerActionType.Remove:
        remove();
        break;
      default:
        break;
    }
  };

  return (
    <TableRow>
      <TableCell>{
        player.actionType === LobbyPlayerActionType.AddGuest
          ? (
            <TextField
              value={guestName}
              onChange={(e) => setGuestName(e.target.value)}
              error={!isValidName}
              helperText={helperText}
              placeholder="Guest player name"
            />
          )
          : player.name
      }
      </TableCell>
      <TableCell>{player.note}</TableCell>
      <TableCell>
        {player.actionType === LobbyPlayerActionType.None || player.actionType === null
          ? <></>
          : (
            <Button
              className={styles.button}
              onClick={onClick}
              style={{ width: '100%' }}
              disabled={player.actionType === LobbyPlayerActionType.AddGuest && !guestName}
            >
              {getActionIcon(player.actionType)}
              {getActionLabel(player.actionType)}
            </Button>
          )}
      </TableCell>
    </TableRow>
  );
};

export default LobbyPlayerRow;
