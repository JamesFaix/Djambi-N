namespace Apex.Api.Db.Mappings

open Apex.Api.Db.Model
open Apex.Api.Model
open System.Linq

[<AutoOpen>]
module SearchMappings =

    let toSearchGame (source : GameSqlModel) (currentUserId : int) : SearchGame =
        let lastEventOn = 
            if source.Events.Any() 
            then source.Events.Max(fun e -> e.CreatedOn) 
            else source.CreatedOn

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
            status = source.GameStatusId
            lastEventOn = lastEventOn
            playerCount = source.Players.Count
            containsMe = source.Players.Any(fun p -> p.UserId.HasValue && p.UserId.Value = currentUserId)
        }
