export default class Routes {
    static readonly base = "/";
    static readonly login = "/login";
    static readonly signup = "/signup";
    static readonly dashboard = "/dashboard";
    static readonly createGame = "/games/create";
    static readonly settings = "/settings";
    static readonly searchGames = "/games/search";

    static game = (gameId : number) => `/games/${gameId}`;
    static readonly gamePattern = "/games/:gameId";

    static lobby = (gameId : number) => `${Routes.game(gameId)}/lobby`;
    static readonly lobbyPattern = `${Routes.gamePattern}/lobby`;

    static play = (gameId : number) => `${Routes.game(gameId)}/play`;
    static readonly playPattern = `${Routes.gamePattern}/play`;

    static diplomacy = (gameId : number) => `${Routes.game(gameId)}/diplomacy`;
    static readonly diplomacyPattern = `${Routes.gamePattern}/diplomacy`;

    static snapshots = (gameId : number) => `${Routes.game(gameId)}/snapshots`;
    static readonly snapshotsPattern = `${Routes.gamePattern}/snapshots`;

    static gameResults = (gameId : number) => `${Routes.game(gameId)}/results`;
    static readonly gameResultsPattern = `${Routes.gamePattern}/results`;

    static readonly rules = "https://github.com/JamesFaix/Apex/wiki/Rules";
}
