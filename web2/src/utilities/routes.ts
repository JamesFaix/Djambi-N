export const createAccount = '/create-account';

export const home = '/home';

export const newGame = '/new-game';

export const rules = '/rules';

export const searchGames = '/search-games';

export const settings = '/settings';

export const signIn = '/sign-in';

export const signOut = '/sign-out';

export const game = (gameId: number): string => `/games/${gameId}`;
export const gameTemplate = '/games/:gameId';

export const gameDiplomacy = (gameId: number): string => `/games/${gameId}/diplomacy`;
export const gameDiplomacyTemplate = '/games/:gameId/diplomacy';

export const gameInfo = (gameId: number): string => `/games/${gameId}/info`;
export const gameInfoTemplate = '/games/:gameId/info';

export const gameLobby = (gameId: number): string => `/games/${gameId}/lobby`;
export const gameLobbyTemplate = '/games/:gameId/lobby';

export const gameOutcome = (gameId: number): string => `/games/${gameId}/outcome`;
export const gameOutcomeTemplate = '/games/:gameId/outcome';

export const gamePlay = (gameId: number): string => `/games/${gameId}/play`;
export const gamePlayTemplate = '/games/:gameId/play';

export const gameSnapshots = (gameId: number): string => `/games/${gameId}/snapshots`;
export const gameSnapshotsTemplate = '/games/:gameId/snapshots';
