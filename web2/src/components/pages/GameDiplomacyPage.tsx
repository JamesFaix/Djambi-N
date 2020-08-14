import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';
import { loadGame } from '../../controllers/gameController';
import { selectActiveGame } from '../../hooks/selectors';

const GameDiplomacyPage: FC<GamePageProps> = ({ gameId }) => {
  const state = useSelector(selectActiveGame);

  useEffect(() => {
    if (state.game === null) {
      loadGame(gameId);
    }
  });

  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} diplomacy page`}
      <br />
      {state.game ? JSON.stringify(state.game) : ''}
    </div>
  );
};

export default GameDiplomacyPage;
