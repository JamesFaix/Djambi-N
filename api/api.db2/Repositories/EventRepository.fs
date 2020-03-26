namespace Apex.Api.Db.Repositories

open System
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Model
open System.Linq
open Microsoft.EntityFrameworkCore
open FSharp.Control.Tasks
open Apex.Api.Db.Mappings
open Apex.Api.Model
open System.ComponentModel

type EventRepository(context : ApexDbContext) =
    let noneOr (f : 'a -> bool) (o : Option<'a>)  =
        o.IsNone || f(o.Value)

    let orderBy (d : ListSortDirection) (f : 'a -> 'b) (xs : IQueryable<'a>) : IOrderedQueryable<'a> =
        if d = ListSortDirection.Descending
        then xs.OrderByDescending(f)
        else xs.OrderBy(f)

    let queryFilter (query : EventsQuery) (event : EventSqlModel) : bool =
        let isAscending = query.direction <> ListSortDirection.Descending

        query.thresholdEventId |> noneOr (fun x -> 
            if isAscending 
            then event.Id <= x 
            else event.Id >= x
        ) &&
        query.thresholdTime |> noneOr (fun x -> 
            if isAscending 
            then event.CreatedOn <= x
            else event.CreatedOn >= x
        )

    interface IEventRepository with
        member __.getEvents (gameId, query) =
            task {
                let maxResults = 
                    match query.maxResults with
                    | Some x -> x
                    | None -> 10000 // TODO: Move to config

                let mutable sqlQuery = 
                    context.Events
                        .Where(fun e -> e.Game.Id = gameId)
                        .Where(queryFilter query)

                sqlQuery <- sqlQuery |> orderBy query.direction (fun e -> e.CreatedOn)

                let! sqlModels =
                    sqlQuery
                        .Take(maxResults)
                        .ToListAsync()
                
                return Ok(sqlModels |> Seq.map toEvent |> Seq.toList)
            }

        member __.persistEvent (request, oldGame, newGame) =
            raise <| NotImplementedException()