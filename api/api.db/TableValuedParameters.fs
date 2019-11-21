namespace Apex.Api.Db

open System
open System.Data
open System.Data.SqlClient
open Dapper
open Apex.Api.Model
open Apex.Api.Common.Json

//See https://medium.com/dapper-net/sql-server-specific-features-2773d894a6ae

module Option =
    let toValueOrDbNull<'a> (x : 'a option) : obj =
        match x with
        | Some value -> value :> obj
        | None -> DBNull.Value :> obj

[<AbstractClass>]
type TvpBase<'a>(xs : 'a seq) =
    let addTableParameter (cmd : IDbCommand) (name : string) (table : DataTable) =
        let p = cmd.CreateParameter() :?> SqlParameter
        p.ParameterName <- name
        p.SqlDbType <- SqlDbType.Structured
        p.Value <- table
        p.TypeName <- if table = null then null else table.TableName
        cmd.Parameters.Add(p) |> ignore
        ()

    abstract member getTable : xs:'a seq -> DataTable

    interface SqlMapper.ICustomQueryParameter with
        member x.AddParameter (cmd, name) =
            let dt = x.getTable xs
            let dt = if dt.Rows.Count > 0 then dt else null
            addTableParameter cmd name dt

type Int32ListTvp(xs : int seq) =
    inherit TvpBase<int>(xs)

    override x.getTable xs =
        let dt = new DataTable()
        dt.TableName <- "Int32List"
        dt.Columns.Add("N", typeof<int>) |> ignore
        for x in xs do
            dt.Rows.Add(x) |> ignore
        dt

type EventListTvp(xs : Event seq) =
    inherit TvpBase<Event>(xs)

    override x.getTable xs =
        let dt = new DataTable()
        dt.TableName <- "EventList"
        dt.Columns.Add("CreatedByUserId", typeof<int>) |> ignore
        dt.Columns.Add("ActingPlayerId", typeof<int>) |> ignore
        dt.Columns.Add("CreatedOn", typeof<DateTime>) |> ignore
        dt.Columns.Add("EventKindId", typeof<byte>) |> ignore
        dt.Columns.Add("EffectsJson", typeof<string>) |> ignore

        for x in xs do
            dt.Rows.Add(
                x.createdBy.userId,
                x.actingPlayerId |> Option.toValueOrDbNull,
                x.createdBy.time,
                Mapping.mapEventKindToId x.kind,
                JsonUtility.serialize x.effects)
            |> ignore
        dt