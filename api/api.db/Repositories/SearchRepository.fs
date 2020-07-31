namespace Apex.Api.Db.Repositories

open System
open System.Data.Entity.Core
open System.Linq
open FSharp.Control.Tasks
open Microsoft.EntityFrameworkCore
open Apex.Api.Db.Interfaces
open Apex.Api.Db.Model
open Apex.Api.Db.Mappings
open Apex.Api.Enums
open Apex.Api.Model

type SearchRepository(context : ApexDbContext) =

    interface ISearchRepository with
        member __.searchGames (query, currentUserId) =
            task {
                let! currentUser =
                    context.Users
                        .Include(fun u -> u.UserPrivileges)
                        .SingleOrDefaultAsync(fun u -> u.UserId = currentUserId)

                if currentUser = null
                // TODO: This should probably 500 not 404
                then raise <| ObjectNotFoundException("User not found.")
                
                // Note: LINQ-to-SQL below
                // Using wordy conditional building of IQueryable rather than higher-order-functions
                // because each lambda is an Expression<Func> not a Func

                let mutable q : IQueryable<GameSqlModel> = 
                    context.Games
                        .Include(fun g -> g.CreatedByUser)
                        .Include(fun g -> g.Players)
                        .Include(fun g -> g.Events)
                        :> IQueryable<GameSqlModel>

                // Security filter
                if not <| currentUser.UserPrivileges.Any(fun p -> p.PrivilegeId = Privilege.ViewGames)
                then 
                    q <- q.Where(fun g -> 
                        g.IsPublic ||
                        g.CreatedByUserId = currentUserId ||
                        g.Players.Any(fun p -> p.UserId = Nullable<int>(currentUserId))
                    )

                // Query filters
                match query.descriptionContains with
                | None -> ()
                | Some x -> 
                    q <- q.Where(fun g -> g.Description.Contains(x))

                match query.createdByUserName with
                | None -> ()
                | Some x -> 
                    q <- q.Where(fun g -> g.CreatedByUser.Name.Contains(x))

                match query.playerUserName with
                | None -> ()
                | Some x -> 
                    q <- q.Where(fun g -> g.Players.Any(fun p -> p.Name.Contains(x)))

                match query.containsMe with
                | None -> ()
                | Some x -> 
                    q <- q.Where(fun g -> g.Players.Any(fun p -> p.UserId = Nullable<int>(currentUserId)) = x)

                match query.isPublic with
                | None -> ()
                | Some x -> 
                    q <- q.Where(fun g -> g.IsPublic = x)

                match query.allowGuests with
                | None -> ()
                | Some x -> 
                    q <- q.Where(fun g -> g.AllowGuests = x)

                match query.createdBefore with
                | None -> ()
                | Some x -> 
                    q <- q.Where(fun g -> g.CreatedOn < x)

                match query.createdAfter with
                | None -> ()
                | Some x -> 
                    q <- q.Where(fun g -> g.CreatedOn > x)

                match query.statuses with
                | [] -> ()
                | xs -> 
                    q <- q.Where(fun g -> xs.Contains(g.GameStatusId))

                match query.lastEventBefore with
                | None -> ()
                | Some x ->
                    q <- q.Where(fun g -> g.Events.OrderBy(fun e -> e.CreatedOn).Last().CreatedOn < x)

                match query.lastEventAfter with
                | None -> ()
                | Some x ->
                    q <- q.Where(fun g -> g.Events.OrderBy(fun e -> e.CreatedOn).Last().CreatedOn > x)

                // Actually get data from SQL
                let! sqlModels = q.ToListAsync()

                // Map results
                let games = 
                    sqlModels
                    |> Seq.map (fun g -> toSearchGame g currentUserId)
                    |> Seq.toList

                return games       
            }