namespace Djambi.Api.IntegrationTests

open Djambi.Api.Db
open Djambi.Api.Logic
open Djambi.Api.Logic.Interfaces

type TestsBase() =
    do
        SqlUtility.connectionString <- connectionString