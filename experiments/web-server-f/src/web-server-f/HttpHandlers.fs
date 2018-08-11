namespace web_server_f

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe
    open web_server_f.Models
    open Djambi.Engine

    let handleGetHello =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = "Hello world, from Giraffe!"
                }
                return! json response next ctx
            }

    let handleStartGame =
        fun (next : HttpFunc) (ctx : HttpContext) -> 
            task {
                let response = Controller.StartGame(["Bilbo"; "Frodo"])

                return! (if response.HasValue
                        then json Controller.GameState next ctx
                        else json response.Error next ctx)
            }