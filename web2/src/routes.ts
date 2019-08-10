export default class Routes {
    static readonly base = "/";
    static readonly login = "/login";
    static readonly signup = "/signup";
    static readonly dashboard = "/dashboard";
    static lobby = (gameId : number) => `/games/${gameId}/lobby`;
    static readonly lobbyPattern = "/games/:gameId/lobby";
    static readonly createGame = "/games/create";
    static play = (gameId : number) => `/games/${gameId}/play`;
    static readonly playPattern = "/games/:gameId/play";
}