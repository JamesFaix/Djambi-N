module Djambi.Api.Logic.Interfaces.Routes

(*
    This module makes more sense at a higher tier of the application.
    It has been placed in this library so that uses of ClientFunctionAttribute can reference it.
 *)

 let private create1<'a> (value : string) : PrintfFormat<'a->obj, obj, obj, obj, 'a> = 
    PrintfFormat<'a->obj, obj, obj, obj, 'a>(value)
    
 let private create2<'a, 'b> (value : string) : PrintfFormat<'a->'b->obj, obj, obj, obj, 'a * 'b> = 
    PrintfFormat<'a->'b->obj, obj, obj, obj, 'a * 'b>(value)

 let private create3<'a, 'b, 'c> (value : string) : PrintfFormat<'a->'b->'c->obj, obj, obj, obj, 'a * 'b * 'c> = 
    PrintfFormat<'a->'b->'c->obj, obj, obj, obj, 'a * 'b *'c>(value)

 [<Literal>]
 let sessions = "/sessions"

 [<Literal>]
 let users = "/users"
  
 [<Literal>]
 let currentUser = "/users/current"

 [<Literal>]
 let user = "/users/%i"
 let userFormat = create1<int>(user)

 [<Literal>]
 let board = "/boards/%i"
 let boardFormat = create1<int>(board)

 [<Literal>]
 let paths = "/boards/%i/cells/%i/paths"
 let pathsFormat = create2<int, int>(paths)

 [<Literal>]
 let games = "/games"

 [<Literal>]
 let game = "/games/%i"
 let gameFormat = create1<int>(game)

 [<Literal>]
 let gamesQuery = "/games/query"

 [<Literal>]
 let gameParameters = "/games/%i/parameters"
 let gameParametersFormat = create1<int>(gameParameters)

 [<Literal>]
 let players = "/games/%i/players"
 let playersFormat = create1<int>(players)

 [<Literal>]
 let player = "/games/%i/players/%i"
 let playerFormat = create2<int, int>(player)

 [<Literal>]
 let playerStatusChange = "/games/%i/players/%i/status/%s"
 let playerStatusChangeFormat = create3<int, int, string>(playerStatusChange)

 [<Literal>]
 let startGame = "/games/%i/start-request"
 let startGameFormat = create1<int>(startGame)

 [<Literal>]
 let selectCell = "/games/%i/current-turn/selection-request/%i"
 let selectCellFormat = create2<int, int>(selectCell)

 [<Literal>]
 let resetTurn = "/games/%i/current-turn/reset-request"
 let resetTurnFormat = create1<int>(resetTurn)

 [<Literal>]
 let commitTurn = "/games/%i/current-turn/commit-request"
 let commitTurnFormat = create1<int>(commitTurn)

 [<Literal>]
 let eventsQuery = "/games/%i/events/query"
 let eventsQueryFormat = create1<int>(eventsQuery)

 [<Literal>]
 let snapshots = "/games/%i/snapshots"
 let snapshotsFormat = create1<int>(snapshots)

 [<Literal>]
 let snapshot = "/games/%i/snapshots/%i"
 let snapshotFormat = create2<int, int>(snapshot)

 [<Literal>]
 let snapshotLoad = "/games/%i/snapshots/%i/load-request"
 let snapshotLoadFormat = create2<int, int>(snapshotLoad)

 [<Literal>]
 let notificationsForCurrentUser = "/notifications"
 
 [<Literal>]
 let notificationsForCurrentUserForGame = "/games/%i/notifications"
 let notificationsForCurrentUserForGameFormat = create1<int>(notificationsForCurrentUserForGame)