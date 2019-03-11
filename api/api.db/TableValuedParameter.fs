namespace Djambi.Api.Db

open System
open System.Data
open System.Data.SqlClient
open Dapper
open Djambi.Api.Model

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
    open Djambi.Api.Common.Json

    let private toValueOrDbNull<'a> (x : 'a option) : obj =
        match x with
        | Some value -> value :> obj
        | None -> DBNull.Value :> obj

    let int32list (xs : int seq) = 
        let dt = new DataTable()
        dt.TableName <- "Int32List"
        dt.Columns.Add("N", typeof<int>) |> ignore
        for x in xs do
            dt.Rows.Add(x) |> ignore            
        new TableValuedParameter(dt)

    let eventList (xs : Event list) =
        let dt = new DataTable()
        dt.TableName <- "EventList"
        dt.Columns.Add("CreatedByUserId", typeof<int>) |> ignore
        dt.Columns.Add("ActingPlayerId", typeof<int>) |> ignore
        dt.Columns.Add("CreatedOn", typeof<DateTime>) |> ignore
        dt.Columns.Add("EventKindId", typeof<byte>) |> ignore
        dt.Columns.Add("EffectsJson", typeof<string>) |> ignore

        for x in xs do
            dt.Rows.Add(
                x.createdByUserId,
                x.actingPlayerId |> toValueOrDbNull,
                x.createdOn,
                Mapping.mapEventKindToId x.kind,
                JsonUtility.serialize x.effects) 
            |> ignore

        new TableValuedParameter(dt)