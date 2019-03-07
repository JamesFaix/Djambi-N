namespace Djambi.Api.Db

open System.Data
open System.Data.SqlClient
open Dapper

//See https://medium.com/dapper-net/sql-server-specific-features-2773d894a6ae

type TableValuedParameter(table : DataTable) =
    interface SqlMapper.ICustomQueryParameter with
        member x.AddParameter(cmd, name) =
            let p = cmd.CreateParameter() :?> SqlParameter
            p.ParameterName <- name
            p.SqlDbType <- SqlDbType.Structured
            p.Value <- table
            p.TypeName <- table.TableName
            cmd.Parameters.Add(p) |> ignore
            ()

module TableValuedParameter =
    let int32list (xs : int seq) = 
        let dt = new DataTable()
        dt.TableName <- "Int32List"
        dt.Columns.Add("N", typeof<int>) |> ignore
        for x in xs do
            dt.Rows.Add(x) |> ignore            
        new TableValuedParameter(dt)