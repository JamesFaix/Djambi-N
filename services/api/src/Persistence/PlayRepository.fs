namespace Djambi.Api.Persistence

type PlayRepository(connectionString : string) =
    inherit RepositoryBase(connectionString)
