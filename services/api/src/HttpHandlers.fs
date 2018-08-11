namespace web_server_f

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe
    open web_server_f.Models

    let handleGetHello =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = "Hello world, from Giraffe!"
                }
                return! json response next ctx
            }
