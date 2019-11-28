module ChickenCheck.Client.Tests.TestHelpers

open Swensen.Unquote
open Expecto
open ChickenCheck.Domain
open ChickenCheck.Client
open Elmish
open System

module Result =
    let okValue res =
        match res with
        | Ok res -> res
        | Error err -> failwithf "Test setup error %A" err

let toAction f arg =
    fun _ -> f arg |> ignore

let toAction2 f arg1 arg2 =
    fun _ -> f arg1 arg2 |> ignore

module Expect =
    let throwsWithType<'texn> f =
        Expect.throwsT<'texn>
            f
            "Should throw with expected type"

    let throwsWithMessages (msgs: string list) f =
        Expect.throwsC
            f
            (fun exn ->
                let failureMessage msg = sprintf "Should contain expected error message: %s" msg
                msgs
                |> List.iter
                    (fun msg ->
                        Expect.stringContains exn.Message msg (failureMessage msg))
                    )

    let throwsWithMessage (msg:string) f = throwsWithMessages [ msg ] f
