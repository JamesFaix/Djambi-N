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
import { PendingGamePlayerSlotViewModel, PendingGamePlayerActionType } from './viewModel';
import { addPlayer, removePlayer } from '../../../controllers/gameController';
import { selectSession } from '../../../hooks/selectors';
import { useFormStyles } from '../../../styles/styles';
import { theme } from '../../../styles/materialTheme';

interface Props {
  player: PendingGamePlayerSlotViewModel,
  game: GameDto
}

const getActionIcon = (action: PendingGamePlayerActionType | null) => {
  switch (action) {
    case PendingGamePlayerActionType.AddGuest:
    case PendingGamePlayerActionType.SelfJoin:
      return <AddIcon />;
    case PendingGamePlayerActionType.Remove:
    case PendingGamePlayerActionType.SelfQuit:
      return <RemoveIcon />;
    default:
      return <></>;
  }
};

const getActionLabel = (action: PendingGamePlayerActionType | null) => {
  switch (action) {
    case PendingGamePlayerActionType.AddGuest:
      return 'Add guest';
    case PendingGamePlayerActionType.SelfJoin:
      return 'Join';
    case PendingGamePlayerActionType.SelfQuit:
      return 'Quit';
    case PendingGamePlayerActionType.Remove:
      return 'Remove';
    default:
      return '';
  }
};

const PendingGamePlayerRow: FC<Props> = ({ player, game }) => {
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
      case PendingGamePlayerActionType.AddGuest:
        addGuest();
        break;
      case PendingGamePlayerActionType.SelfJoin:
        selfJoin();
        break;
      case PendingGamePlayerActionType.SelfQuit:
      case PendingGamePlayerActionType.Remove:
        remove();
        break;
      default:
        break;
    }
  };

  return (
    <TableRow>
      <TableCell>{
        player.actionType === PendingGamePlayerActionType.AddGuest
          ? (
            <TextField
              value={guestName}
              onChange={(e) => setGuestName(e.target.value)}
              error={!isValidName}
              helperText={helperText}
            // className={styles.control}
            />
          )
          : player.name
      }
      </TableCell>
      <TableCell>{player.note}</TableCell>
      <TableCell>
        {/* <ListItem button onClick={onClick}>
          <ListItemIcon>{getActionIcon(player.actionType)}</ListItemIcon>
          <ListItemText primary={getActionLabel(player.actionType)} />
        </ListItem> */}
        {player.actionType === PendingGamePlayerActionType.None || player.actionType === null
          ? <></>
          : (
            <Button
              className={styles.button}
              onClick={onClick}
              style={{ width: '100%' }}
            >
              {getActionIcon(player.actionType)}
              {getActionLabel(player.actionType)}
            </Button>
          )}
      </TableCell>
    </TableRow>
  );
};

export default PendingGamePlayerRow;
