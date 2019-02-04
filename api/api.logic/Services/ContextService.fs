module Djambi.Api.Logic.ContextService

open Djambi.Api.Model

let getActingPlayerId (session : Session) (game : Game) = 
    //Before the game starts, acting players are obvious or can be determined by userID.
    //Because players can be deleted at this point, we don't want to create a foriegn key reference to them.
    if game.status <> GameStatus.Started then None
    else
        let sessionPlayers = game.players |> List.filter (fun p -> p.userId = Some session.user.id)    
        if sessionPlayers.IsEmpty then None //This would be the case for an admin.
        else 
            let currentPlayerId = game.turnCycle.Head
            if sessionPlayers |> List.exists (fun p -> p.id = currentPlayerId) then
                Some currentPlayerId
            else    
                None //This would be the case for an admin that is also a player in the game, and doing something when its not their turn.