import React, { FC } from 'react';
import {
  Table, TableContainer, TableHead, TableCell, TableRow, TableBody,
} from '@material-ui/core';
import { useSelector } from 'react-redux';
import { selectActiveGame, selectSession } from '../../../hooks/selectors';
import LobbyPlayerRow from './LobbyPlayerRow';
import { getViewModels } from './viewModel';

const LobbyPlayersTable: FC = () => {
  const { game } = useSelector(selectActiveGame);
  const { user } = useSelector(selectSession);

  if (game === null || user === null) {
    return <></>;
  }

  const viewModels = getViewModels(user, game);

  return (
    <TableContainer>
      <Table>
        <colgroup>
          <col style={{ width: '33%' }} />
          <col style={{ width: '33%' }} />
          <col style={{ width: '33%' }} />
        </colgroup>
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Note</TableCell>
            <TableCell>Action</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {
            viewModels.map((p, i) => (
              <LobbyPlayerRow
                key={i.toString()}
                player={p}
                game={game}
              />
            ))
          }
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default LobbyPlayersTable;
