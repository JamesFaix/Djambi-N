namespace Djambi.Api.Persistence

type RepositoryRegistry =
    {
        lobby : LobbyRepository
        gameStart : GameStartRepository
        play : PlayRepository
    }