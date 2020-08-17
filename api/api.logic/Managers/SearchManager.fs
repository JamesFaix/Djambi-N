namespace Djambi.Api.Logic.Managers

open Djambi.Api.Db.Interfaces
open Djambi.Api.Logic.Interfaces

type SearchManager(searchRepo : ISearchRepository) =
    interface ISearchManager with        
        member __.searchGames query session =
            searchRepo.searchGames (query, session.user.id)