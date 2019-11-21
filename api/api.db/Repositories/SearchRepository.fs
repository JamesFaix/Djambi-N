namespace Apex.Api.Db.Repositories

open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Db
open Apex.Api.Db.Interfaces

type SearchRepository(ctxProvider : CommandContextProvider) =
    interface ISearchRepository with
        member x.searchGames (query, currentUserId) =
            Commands2.searchGames (query, currentUserId)
            |> Command.execute ctxProvider
            |> thenMap (List.map Mapping.mapSearchGameResponse)
