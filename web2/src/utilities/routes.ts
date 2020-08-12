export const createAccount = '/create-account';

export const home = '/home';

export const newGame = '/new-game';

export const rules = '/rules';

export const searchGames = '/search-games';

export const settings = '/settings';

export const signIn = '/sign-in';

export const signOut = '/sign-out';

export const gameDiplomacy = (gameId: number): string => `/games/${gameId}/diplomacy`;

export const gameLobby = (gameId: number): string => `/games/${gameId}/lobby`;

export const gameOutcome = (gameId: number): string => `/games/${gameId}/outcome`;

export const gamePlay = (gameId: number): string => `/games/${gameId}/play`;

export const gameSnapshots = (gameId: number): string => `/games/${gameId}/snapshots`;
