import * as Sprintf from 'sprintf-js';

export default class Routes {

    public createGame() { return "/games/create"; }

    public dashboard() { return "/dashboard"; }

    public findGame() { return "/games/find"; }

    public game(gameId : number) { return Sprintf.sprintf("/games/%i", gameId); }

    public gameTemplate() { return "/games/:gameId"; }

    public gameInfo(gameId : number) { return Sprintf.sprintf("/games/%i/info", gameId); }

    public gameInfoTemplate() { return "/games/:gameId/info"; }

    public home() { return "/"; }

    public login() { return "/login"; }

    public myGames() { return "/games/my"; }

    public rules() { return "https://github.com/GamesFaix/Djambi3/blob/master/docs/Rules.md"; }

    public signup() { return "/signup"; }

    public snapshots(gameId : number) { return Sprintf.sprintf("/games/%i/snapshots", gameId); }

    public snapshotsTemplate() { return "/games/:gameId/snapshots"; }
}