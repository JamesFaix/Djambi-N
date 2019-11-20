namespace Apex.Api.Web.Controllers

open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Model
open Apex.Api.Web.Interfaces
open Apex.Api.Web
open Apex.Api.Logic.Interfaces

type SearchController(searchMan : ISearchManager,
                      u : HttpUtility) =
    interface ISearchController with
        member x.searchGames =
            let func ctx =
                u.getSessionAndModelFromContext<GamesQuery> ctx
                |> thenBindAsync (fun (jsonModel, session) -> searchMan.searchGames jsonModel session)
            u.handle func