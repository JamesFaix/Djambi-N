namespace Djambi.Api.Persistence

open System.Threading.Tasks

open Dapper
open Giraffe
open Newtonsoft.Json

open Djambi.Api.Domain.PlayModels

type PlayRepository(connectionString) =
    inherit RepositoryBase(connectionString)
    