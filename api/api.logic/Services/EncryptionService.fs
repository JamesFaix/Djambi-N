namespace Djambi.Api.Logic.Services

open System
open System.Linq
open System.Security.Cryptography
open Djambi.Api.Model.UserModel
open Djambi.Api.Logic.Interfaces

// Based on this example
// https://medium.com/dealeron-dev/storing-passwords-in-net-core-3de29a3da4d2

type EncryptionService() =
    
    let saltBytes = 128(*bits*) / 8
    let iterations = 1000;
    let algorithmName = HashAlgorithmName.SHA256
    let keySize = 8;

    interface IEncryptionService with
        member __.hash password =
            use alg = new Rfc2898DeriveBytes(
                        password, 
                        saltBytes,
                        iterations,
                        algorithmName)
            let key = alg.GetBytes keySize |> Convert.ToBase64String
            let salt = alg.Salt |> Convert.ToBase64String
            sprintf "%i.%s.%s" iterations salt key

        member __.check (hash, password) =
            let parts = hash.Split('.');
        
            if parts.Length <> 3
            then raise <| FormatException("Incorrect hash format")

            let its = parts.[0] |> Convert.ToInt32
            let salt = parts.[1] |> Convert.FromBase64String
            let key = parts.[2] |> Convert.FromBase64String

            let needsUpgrade = its <> iterations

            use alg = new Rfc2898DeriveBytes(
                        password,
                        salt,
                        iterations,
                        algorithmName)

            let keyToCheck = alg.GetBytes keySize
            let verified = keyToCheck.SequenceEqual key

            {
                verified = verified
                needsUpgrade = needsUpgrade
            }