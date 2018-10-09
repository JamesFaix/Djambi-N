module Djambi.Api.ContractTests.SessionTests

open System
open System.Net
open System.Net.Http
open System.Text.RegularExpressions
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Web.Model.LobbyWebModel

let getToken (cookie : string) : string option =
    let m = Regex.Match(cookie, "^DjambiSession=(.*?);");
    match m.Groups.Count with
    | 0 -> None
    | _ -> Some m.Groups.[1].Value

[<Fact>]
let ``POST sessions should work``() =
    task {
        //Arrange
        let createUserRequest = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }
    
        let! _ = WebUtility.sendRequest("/users", HttpMethod.Post, createUserRequest)

        //Act
        let request = 
            {
                userName = createUserRequest.name
                password = createUserRequest.password
            }
    
        let! response = WebUtility.sendRequest<LoginRequestJsonModel, SessionResponseJsonModel>(
                            "/sessions", HttpMethod.Post, request)
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.statusCode)

        Assert.True(response.headers.ContainsKey("Set-Cookie"))
        let token = getToken response.headers.["Set-Cookie"]
        Assert.True(token.IsSome)

        Assert.NotEqual(0, response.value.id)
        Assert.Equal(1, response.value.userIds.Length)
    }

[<Fact>]
let ``POST user should work`` () =
    task {
        //Arrange
        let request = 
            {
                name = Guid.NewGuid().ToString()
                password = "test"
                role = "Normal"
            }
    
        //Act
        let! response = WebUtility.sendRequest<CreateUserJsonModel, UserJsonModel>(
                            "/users", HttpMethod.Post, request)
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.statusCode)
                
        Assert.NotEqual(0, response.value.id)
        Assert.Equal(request.name, response.value.name)
        Assert.Equal(request.role, response.value.role)
    }