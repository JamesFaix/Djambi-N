namespace Djambi.Api.IntegrationTests

open Djambi.Api.Db

type TestsBase() =
    do 
        SqlUtility.connectionString <- connectionString