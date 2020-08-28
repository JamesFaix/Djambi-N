import React, { FC } from 'react';
import {
  Table, TableContainer, TableCell, TableRow, TableBody, TableHead,
} from '@material-ui/core';
import { SearchGameDto } from '../../api-client';
import * as Routes from '../../utilities/routes';
import { navigateTo } from '../../controllers/navigationController';

interface Props {
  games: SearchGameDto[]
}

const GameSearchResultsTable: FC<Props> = ({ games }) => {
  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>#</TableCell>
            <TableCell>Description</TableCell>
            <TableCell>Status</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {
            games.map((g, i) => (
              <TableRow key={i.toString()} onClick={() => navigateTo(Routes.game(g.id))}>
                <TableCell>{g.id}</TableCell>
                <TableCell>{g.parameters.description}</TableCell>
                <TableCell>{g.status}</TableCell>
              </TableRow>
            ))
          }
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default GameSearchResultsTable;
