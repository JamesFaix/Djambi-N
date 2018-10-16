module Djambi.Api.Logic.Services.GameStartService

open System
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model.BoardModel
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.PlayModel

let addVirtualPlayers(lobby : LobbyWithPlayers) : LobbyWithPlayers AsyncHttpResult =
    let missingPlayerCount = lobby.regionCount - lobby.players.Length

    let getVirtualNamesToUse (possibleNames : string list) = 
        Enumerable.Except(
            possibleNames, 
            lobby.players |> Seq.map (fun p -> p.name), 
            StringComparer.OrdinalIgnoreCase) 
        |> Utilities.shuffle
        |> Seq.take missingPlayerCount

    if missingPlayerCount = 0
    then lobby |> okTask
    else
        LobbyRepository.getVirtualPlayerNames()
        |> thenMap getVirtualNamesToUse
        |> thenDoEachAsync (fun name -> 
            let request = CreatePlayerRequest.``virtual`` (lobby.id, name)
            LobbyRepository.addPlayerToLobby request
            |> thenMap ignore
        )
        |> thenBindAsync (fun _ -> LobbyRepository.getLobbyWithPlayers lobby.id)

let getStartingConditions(players : LobbyPlayer list) : PlayerStartConditions list =
    let colorIds = [0..(Constants.maxRegions-1)] |> Utilities.shuffle |> Seq.take players.Length
    let regions = [0..(players.Length-1)] |> Utilities.shuffle
    let turnOrder = players |> Utilities.shuffle

    turnOrder 
    |> Seq.zip3 colorIds regions
    |> Seq.mapi (fun i (c, r, p) -> 
        {
            playerId = p.id
            turnNumber = i
            region = r
            color = c
        })
    |> Seq.toList
 
let createPieces(board : BoardMetadata, startingConditions : PlayerStartConditions list) : Piece list =
    let createPlayerPieces(board : BoardMetadata, player : PlayerStartConditions, startingId : int) : Piece list =            
        let getPiece(id : int, pieceType: PieceType, x : int, y : int) =
            {
                id = id
                pieceType = pieceType
                playerId = Some player.playerId
                originalPlayerId = player.playerId
                cellId = board.cellAt({ x = x; y = y; region = player.region}).id
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
        
    startingConditions
    |> List.mapi (fun i cond -> createPlayerPieces(board, cond, i*Constants.piecesPerPlayer))
    |> List.collect id

let startGame(lobbyId : int) : StartGameResponse AsyncHttpResult =
    LobbyRepository.getLobbyWithPlayers lobbyId
    |> thenBindAsync addVirtualPlayers
    |> thenMap (fun lobby -> 
        let startingConditions = getStartingConditions lobby.players
        let board = BoardModelUtility.getBoardMetadata lobby.regionCount
        let pieces = createPieces(board, startingConditions)
        let gameState : GameState = 
            {
                players = lobby.players 
                            |> List.map (fun p -> 
                            { 
                                id = p.id
                                isAlive = true
                            })
                pieces = pieces
                turnCycle = startingConditions 
                            |> List.sortBy (fun cond -> cond.turnNumber)
                            |> List.map (fun cond -> cond.playerId)
            }                

        let gameWithoutSelectionOptions : Game = 
            {
                regionCount = lobby.regionCount
                gameState = gameState
                turnState = 
                    {
                        status = AwaitingSelection
                        selections = List.empty
                        selectionOptions = List.empty
                        requiredSelectionType = Some Subject
                    }
            }

        let (selectionOptions, _) = PlayService.getSelectableCellsFromState gameWithoutSelectionOptions
           
        {
            lobbyId = lobby.id
            startingConditions = startingConditions
            gameState = gameState
            turnState = { gameWithoutSelectionOptions.turnState with selectionOptions = selectionOptions }
        }
    )
    |> thenBindAsync (fun startRequest -> 
        PlayRepository.startGame startRequest
        |> thenMap (fun gameId -> 
            {
                gameId = gameId
                startingConditions = startRequest.startingConditions
                gameState = startRequest.gameState
                turnState = startRequest.turnState
            }
        )
    )