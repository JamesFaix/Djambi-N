namespace Apex.Api.Db.Repositories

open System
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Model
open FSharp.Control.Tasks
open Apex.Api.Db.Mappings
open System.Linq
open Apex.Api.Model
open Apex.Api.Common.Control
open Microsoft.EntityFrameworkCore

type SearchRepository(context : ApexDbContext) =
    let privId = Privilege.ViewGames |> toPrivilegeSqlId

    let securityFilter (currentUser : UserSqlModel) (game : GameSqlModel) : bool =
        game.IsPublic ||
        game.CreatedByUser.UserId = currentUser.UserId ||
        game.Players.Any(fun p -> p.User.UserId = currentUser.UserId) ||
        currentUser.UserPrivileges.Any(fun p -> p.PrivilegeId = privId) 

    let comparison = StringComparison.InvariantCultureIgnoreCase

    let noneOr (f : 'a -> bool) (o : Option<'a>)  =
        o.IsNone || f(o.Value)

    let queryFilter (query : GamesQuery) (currentUser : UserSqlModel) (game : GameSqlModel) : bool =
        query.descriptionContains |> noneOr (fun x -> 
            game.Description.Contains(x, comparison))
        &&        
        query.createdByUserName |> noneOr (fun x -> 
            game.CreatedByUser.Name.Contains(x, comparison))
        &&        
        query.playerUserName |> noneOr (fun x -> 
            game.Players.Any(fun p -> 
                p.User <> null && 
                p.User.Name.Contains(x, comparison)
            )
        ) &&
        query.containsMe |> noneOr (fun x -> 
            x = game.Players.Any(fun p -> 
                p.User <> null && 
                p.User.UserId = currentUser.UserId
            )
        ) &&
        query.isPublic |> noneOr (fun x -> x = game.IsPublic) &&        
        query.allowGuests |> noneOr (fun x -> x = game.AllowGuests) && 
        (
           query.statuses.IsEmpty ||
           query.statuses 
           |> Seq.map toGameStatusSqlId 
           |> Seq.contains game.GameStatus.Id
        ) &&
        query.createdBefore |> noneOr (fun x -> x > game.CreatedOn) &&        
        query.createdAfter |> noneOr (fun x -> x < game.CreatedOn) &&
        (
            let lastEvent = game.Events.OrderByDescending(fun e -> e.CreatedOn).First()
            query.lastEventBefore |> noneOr (fun x -> x > lastEvent.CreatedOn) &&
            query.lastEventAfter |> noneOr (fun x -> x > lastEvent.CreatedOn)        
        )

    interface ISearchRepository with
        member __.searchGames (query, currentUserId) =
            task {
                let! currentUser = context.Users.FindAsync(currentUserId)
                if currentUser = null
                then raise <| HttpException(404, "Not found.")

                let games = 
                    context.Games
                        .Include(fun g -> g.Players)
                        .Include(fun g -> g.Events)
                        .AsEnumerable()
                        |> Seq.filter (securityFilter currentUser)
                        |> Seq.filter (queryFilter query currentUser)
                        |> Seq.map (fun g -> toSearchGame g currentUserId)
                        |> Seq.toList

                return Ok(games)            
            }