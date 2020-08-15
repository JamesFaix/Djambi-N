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

const PendingGamePlayerRow: FC<Props> = ({ player, game }) => {
  const controlStyle = {
    padding: '10px',
  };

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

  const isBlank = guestName === '';
  if (isBlank) {
    helperText = 'Name cannot be blank';
  }

  const isValidName = !isDuplicateName && !isBlank;

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
              style={controlStyle}
            />
          )
          : player.name
      }
      </TableCell>
      <TableCell>{player.note}</TableCell>
      <TableCell>
        <Button
          style={controlStyle}
          onClick={onClick}
        >
          {getActionIcon(player.actionType)}
        </Button>
      </TableCell>
    </TableRow>
  );
};

export default PendingGamePlayerRow;
