namespace Djambi.Api.Logic.Managers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Interfaces
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type SearchManager(searchRepo : ISearchRepository,
                   securityServ : SecurityService) =
    interface ISearchManager with        
        member x.searchGames query session =
            searchRepo.searchGames query
            |> thenMap (fun games ->
                if session.user.has ViewGames
                then games
                else games |> List.filter (securityServ.isGameViewableByActiveUser session)
            )