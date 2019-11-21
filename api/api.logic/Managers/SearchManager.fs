namespace Apex.Api.Logic.Managers

open Apex.Api.Db.Interfaces
open Apex.Api.Logic.Interfaces

type SearchManager(searchRepo : ISearchRepository) =
    interface ISearchManager with        
        member x.searchGames query session =
            searchRepo.searchGames (query, session.user.id)