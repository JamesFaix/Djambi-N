namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces

type SearchRepository(ctxProvider : CommandContextProvider) =
    interface ISearchRepository with
        member x.searchGames (query, currentUserId) =
            Commands2.searchGames (query, currentUserId)
            |> Command.execute ctxProvider
            |> thenMap (List.map Mapping.mapSearchGameResponse)
