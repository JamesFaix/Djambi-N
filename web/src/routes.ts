export default class Routes {

    public createGame() { return "/games/create"; }

    public dashboard() { return "/dashboard"; }

    public findGame() { return "/games/find"; }

    public game(gameId : number) { return `/games/${gameId}`; }

    public gameTemplate() { return "/games/:gameId"; }

    public gameInfo(gameId : number) { return `/games/${gameId}/info`; }

    public gameInfoTemplate() { return "/games/:gameId/info"; }

    public home() { return "/"; }

    public login() { return "/login"; }

    public myGames() { return "/games/my"; }

    public rules() { return "https://github.com/GamesFaix/Djambi3/wiki/Rules"; }

    public signup() { return "/signup"; }

    public snapshots(gameId : number) { return `/games/${gameId}/snapshots`; }

    public snapshotsTemplate() { return "/games/:gameId/snapshots"; }
}