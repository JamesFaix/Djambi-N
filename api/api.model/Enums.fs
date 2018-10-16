module Djambi.Api.Model.Enums

type GameStatus =
    | Open
    | Started
    | Complete
    | Cancelled
        
type PlayerType =
    | User
    | Guest
    | Virtual