module Djambi.Api.Logic.Services.GameStartService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model.BoardModel
open Djambi.Api.Model.GameModel
open Djambi.Api.Model.PlayerModel
open Djambi.Api.Model.SessionModel
open Djambi.Api.Logic.Services

let getStartingConditions(players : Player list) : PlayerStartConditions list =
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

let startGame (lobbyId : int) (session : Session) : StartGameResponse AsyncHttpResult =
    LobbyRepository.getLobby(lobbyId, session.userId)
    |> thenBind (fun lobby -> 
        if session.isAdmin || session.userId = lobby.createdByUserId
        then Ok lobby
        else Error <| HttpException(403, "Cannot start game from lobby created by another user.")
    )
    |> thenBindAsync (fun lobby -> 
        PlayerRepository.getPlayers lobbyId
        |> thenBindAsync (PlayerService.fillEmptyPlayerSlots lobby)
        |> thenMap (fun players -> lobby.addPlayers players)
    )
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

        let (selectionOptions, _) = GameService.getSelectableCellsFromState gameWithoutSelectionOptions
           
        {
            lobbyId = lobby.id
            startingConditions = startingConditions
            gameState = gameState
            turnState = { gameWithoutSelectionOptions.turnState with selectionOptions = selectionOptions }
        }
    )
    |> thenBindAsync (fun startRequest -> 
        GameRepository.startGame startRequest
        |> thenMap (fun gameId -> 
            {
                gameId = gameId
                startingConditions = startRequest.startingConditions
                gameState = startRequest.gameState
                turnState = startRequest.turnState
            }
        )
    )