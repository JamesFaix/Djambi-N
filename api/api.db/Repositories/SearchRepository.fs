namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces

type SearchRepository(ctxProvider : CommandContextProvider) =
    interface ISearchRepository with
        member x.searchGames query =
            Commands2.searchGames query
            |> Command.execute ctxProvider
            |> thenMap (List.map Mapping.mapGameResponse)
