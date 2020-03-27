namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open Apex.Api.Db.Mappings
open System.Linq

[<AutoOpen>]
module SearchMappings =

    let toSearchGame (source : GameSqlModel) (currentUserId : int) : SearchGame =
        {
            id = source.GameId
            parameters = {
                description = source.Description |> Option.ofObj
                allowGuests = source.AllowGuests
                isPublic = source.IsPublic
                regionCount = int source.RegionCount
            }
            createdBy = {
                userId = source.CreatedByUser.UserId
                userName = source.CreatedByUser.Name
                time = source.CreatedOn
            }
            status = source.GameStatusId |> toGameStatus
            lastEventOn = source.Events.Max(fun e -> e.CreatedOn)
            playerCount = source.Players.Count
            containsMe = source.Players.Any(fun p -> p.UserId.HasValue && p.UserId.Value = currentUserId)
        }
