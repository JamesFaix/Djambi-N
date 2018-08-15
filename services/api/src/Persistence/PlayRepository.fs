namespace Djambi.Api.Persistence

type PlayRepository(connectionString : string) =
    inherit RepositoryBase(connectionString)

    //member this.getOpenGames() : GameMetadata seq Task =
    //    let query = "SELECT g.GameId, g.Description AS GameDescription, g.BoardRegionCount,
    //                    u.UserId, u.Name as UserName \
    //                 FROM Games g \
    //                    INNER JOIN Players p ON g.GameId = p.GameId \
    //                    INNER JOIN Users u ON u.UserId = p.UserId \
    //                 WHERE g.Status = 1" //1 = Open
    //    task {
    //        use cn = this.getConnection()
    //        let! sqlModels = cn.QueryAsync<OpenGamePlayerSqlModel>(query)
    //        let models = sqlModels 
    //                     |> Seq.groupBy (fun sql -> sql.gameId)
    //                     |> Seq.map (fun grp -> 
    //                        {
                                    
    //                        })


    //        return ()            
    //    }