import React, { FC } from 'react';
import {
  Table, TableContainer, TableCell, TableRow, TableBody, TableHead, makeStyles,
} from '@material-ui/core';
import { SearchGameDto } from '../../api-client';
import * as Routes from '../../utilities/routes';
import { navigateTo } from '../../controllers/navigationController';

interface Props {
  games: SearchGameDto[]
}

const GameSearchResultsTable: FC<Props> = ({ games }) => {
  const classes = makeStyles({
    row: {
      background: '#000000',
      '&:hover': {
        background: '#555555',
      },
    },
  })();

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
              <TableRow
                className={classes.row}
                key={i.toString()}
                onClick={() => navigateTo(Routes.game(g.id))}
              >
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
