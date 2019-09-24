namespace Djambi.Api.Web.Controllers

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web
open Djambi.Api.Logic.Interfaces

type SearchController(searchMan : ISearchManager,
                      u : HttpUtility) =
    interface ISearchController with
        member x.searchGames =
            let func ctx =
                u.getSessionAndModelFromContext<GamesQuery> ctx
                |> thenBindAsync (fun (jsonModel, session) -> searchMan.searchGames jsonModel session)
            u.handle func