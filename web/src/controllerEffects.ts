import { User, Game, GameStatus } from "./api/model";
import Controller from "./controller";
import { useEffect } from "react";
import Routes from "./routes";

export default class ControllerEffects {
    public static redirectToDashboardIfLoggedIn(user : User) : void {
        return useEffect(() => {
            if (user) {
                Controller.Session.redirectToDashboardIfLoggedIn();
            }
        });
    }

    public static redirectToLobbyIfNotGameStatus(game : Game, status : GameStatus) : void {
        return useEffect(() => {
            if (!game) { return; }
            if (game.status !== status) {
                Controller.navigateTo(Routes.lobby(game.id));
            }
        });
    }

    public static redirectToLoginIfNotLoggedIn(user : User) : void {
        return useEffect(() => {
            if (!user) {
                Controller.Session.redirectToLoginIfNotLoggedIn();
            }
        });
    }

    public static redirectToGame(routeGameId : number, activeGame : Game) : void {
        if (activeGame && activeGame.id === routeGameId) {
            const url = ControllerEffects.getDefaultGameUrl(routeGameId, activeGame.status);
            Controller.navigateTo(url);
        }
    }

    private static getDefaultGameUrl(gameId : number, status : GameStatus) : string {
        switch (status) {
            case GameStatus.Canceled:
            case GameStatus.Pending:
                return Routes.lobby(gameId);
            case GameStatus.Over:
                return Routes.gameOver(gameId);
            case GameStatus.InProgress:
                return Routes.play(gameId);
            default:
                return null;
        }
    }

    public static redirectToLoginOrDashboardPage(user : User) : void {
        return useEffect(() => {
            if (user) {
                Controller.navigateTo(Routes.dashboard);
            } else {
                Controller.navigateTo(Routes.login);
            }
        });
    }
}