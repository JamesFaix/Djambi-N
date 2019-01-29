export default class Routes {

    public static createGame() { return "/games/create"; }

    public static dashboard() { return "/dashboard"; }

    public static findGame() { return "/games/find"; }

    public static game(gameId : number) { return "/games/" + gameId; }

    public static gameTemplate() { return "/games/:gameId"; }

    public static gameInfo(gameId : number) { return "/games/" + gameId + "/info"; }

    public static gameInfoTemplate() { return "/games/:gameId/info"; }

    public static home() { return "/"; }

    public static login() { return "/login"; }

    public static myGames() { return "/games/my"; }

    public static rules() { return "https://github.com/GamesFaix/Djambi3/blob/master/docs/Rules.md"; }

    public static signup() { return "/signup"; }
}