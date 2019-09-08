namespace Djambi.Api.Logic.Services

open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Logic

type GameStartService(playerServ : PlayerService,
                      selectionOptionsServ : SelectionOptionsService) =

    member x.getGameStartEvent (game : Game) (session : Session) : CreateEventRequest AsyncHttpResult =
        Security.ensureCreatorOrEditPendingGames session game
        |> Result.bindAsync (fun _ ->
            if game.players
                |> List.filter (fun p -> p.kind <> Neutral)
                |> List.length = 1
            then errorTask <| HttpException(400, "Cannot start game with only one player.")
            elif game.status <> GameStatus.Pending
            then errorTask <| HttpException(400, "Cannot start game unless it is pending.") 
            else
                playerServ.fillEmptyPlayerSlots game
                |> thenMap (fun addNeutralPlayerEffects ->
                    let effects =
                        //The order is very important for effect processing. Neutral players must be created before the game start.
                        List.append
                            addNeutralPlayerEffects
                            [Effect.GameStatusChanged { oldValue = GameStatus.Pending; newValue = InProgress }]

                    {
                        kind = EventKind.GameStarted
                        effects = effects
                        createdByUserId = session.user.id
                        actingPlayerId = Context.getActingPlayerId session game
                    }
                )
        )

    member x.assignStartingConditions(players : Player list) : Player list =
        let colorIds = [0..(Constants.maxRegions-1)] |> List.shuffle |> Seq.take players.Length
        let regions = [0..(players.Length-1)] |> List.shuffle

        let playersWithAssignments =
            players
            |> Seq.zip3 colorIds regions
            |> Seq.map (fun (c, r, p) ->
                {
                    p with
                        startingRegion = Some r
                        startingTurnNumber = None
                        colorId = Some c
                }
            )

        (*
            At this point, neutral players may still have ID = 0, because
            they have not yet been persisted as part of the StartGame transaction.
            Index on name instead of id.
        *)
        let dict = Enumerable.ToDictionary (playersWithAssignments, (fun p -> p.name))

        let nonNeutralPlayers =
            players
            |> List.filter (fun p -> p.kind <> Neutral)
            |> List.shuffle
            |> Seq.mapi (fun i p -> (i, p))

        for (i, p) in nonNeutralPlayers do
            dict.[p.name] <- { dict.[p.name] with startingTurnNumber = Some i }

        dict.Values
        |> Seq.map (fun p ->
            let status =  if p.kind = Neutral then AcceptsDraw else Alive
            { p with status = status }
        )
        |> Seq.toList

    member x.createPieces(board : BoardMetadata, players : Player list) : Piece list =
        let createPlayerPieces(board : BoardMetadata, player : Player, startingId : int) : Piece list =
            let getPiece(id : int, pieceType: PieceKind, x : int, y : int) =
                {
                    id = id
                    kind = pieceType
                    playerId = Some player.id
                    originalPlayerId = player.id
                    cellId = board.cellAt({x = x; y = y; region = player.startingRegion.Value}).id
                }
            let n = Constants.regionSize - 1
            [
                getPiece(startingId, Chief, n,n)
                getPiece(startingId+1, Reporter, n,n-1)
                getPiece(startingId+2, Assassin, n-1,n)
                getPiece(startingId+3, Diplomat, n-1,n-1)
                getPiece(startingId+4, Gravedigger, n-2,n-2)
                getPiece(startingId+5, Thug, n-2,n-1)
                getPiece(startingId+6, Thug, n-2,n)
                getPiece(startingId+7, Thug, n-1,n-2)
                getPiece(startingId+8, Thug, n,n-2)
            ]

        players
        |> List.mapi (fun i cond -> createPlayerPieces(board, cond, i*Constants.piecesPerPlayer))
        |> List.collect id

    member x.applyStartGame (game : Game) : Game =
        let board = BoardModelUtility.getBoardMetadata game.parameters.regionCount
        let players = x.assignStartingConditions game.players

        let game =
            {
                game with
                    status = InProgress
                    pieces = x.createPieces(board, players) //Starting conditions must first be assigned
                    players = players
                    turnCycle = players //Starting conditions must first be assigned
                        |> List.filter (fun p -> p.startingTurnNumber.IsSome)
                        |> List.sortBy (fun p -> p.startingTurnNumber.Value)
                        |> List.map (fun p -> p.id)
                    currentTurn = Some Turn.empty
            }

        let options = (selectionOptionsServ.getSelectableCellsFromState game) |> Result.value
        let turn = { game.currentTurn.Value with selectionOptions = options }
        { game with  currentTurn =  Some turn }

    member x.getCorrectNeutralPiecePlayerIdsEvent (game : Game) (session : Session) : CreateEventRequest AsyncHttpResult =
        let neutralPlayers = game.players |> List.filter (fun p -> p.kind = Neutral)
        let neutralPieces = 
            game.pieces 
            |> List.groupBy (fun p -> p.playerId) 
            |> List.filter(fun (pId, _) -> pId.IsSome && pId.Value < 0)

        //Just zipping in arbitrary order, disregarding which placeholder IDs had been assigned to players originally
        let zipped = 
            Seq.zip neutralPlayers neutralPieces 
            |> Seq.map (fun (player, (_, pieces)) -> (player, pieces))

        let effects =
            zipped
            |> Seq.collect (fun (player, pieces) -> 
                pieces 
                |> Seq.map (fun p -> 
                    let f : PieceEnlistedEffect = {
                        oldPiece = p
                        newPlayerId = player.id
                    }
                    Effect.PieceEnlisted f
                )
            )
            |> Seq.toList

        okTask <|
        {
            kind = EventKind.CorrectNeutralPiecePlayerIds
            effects = effects
            createdByUserId = session.user.id
            actingPlayerId = Context.getActingPlayerId session game
        }
        