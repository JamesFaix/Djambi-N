namespace Apex.Api.Db

open System.Data
open System.Data.SqlClient
open System
open Microsoft.Extensions.Options
open Apex.Api.Model.Configuration

/// <summary>
/// Encapsulates a database connection, with optional transaction.
/// </summary>
type CommandContext(cn, tran) =
    member x.connection : IDbConnection = cn
    member x.transaction : IDbTransaction option = tran

    member x.commit () =
        match x.transaction with
        | Some t -> t.Commit()
        | _ -> ()

    member x.rollback () =
        match x.transaction with
        | Some t -> t.Rollback()
        | _ -> ()

    interface IDisposable with
        member x.Dispose() =
            match x.transaction with
            | Some t -> t.Dispose()
            | _ -> ()
            x.connection.Dispose()

/// <summary>
/// Creates <c>CommandContext</c> instances.
/// </summary>
type CommandContextProvider(options : IOptions<SqlSettings>) =
    let connectionString = options.Value.connectionString

    member x.getContext() : CommandContext =
        let cn = new SqlConnection(connectionString)
        cn.Open()
        new CommandContext(cn, None)

    member x.getContextWithTransaction() : CommandContext =
        let cn = new SqlConnection(connectionString)
        cn.Open()
        let tr = cn.BeginTransaction() :> IDbTransaction
        new CommandContext(cn, Some tr)