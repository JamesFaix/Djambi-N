namespace Djambi.Api.IntegrationTests.Logic.UserService

open Djambi.Api.IntegrationTests

type GetUsersTests() =
    inherit TestsBase()

    //Get users should work if admin

    //Get users should fail if not admin