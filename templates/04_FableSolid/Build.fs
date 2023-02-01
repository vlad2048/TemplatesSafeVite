open Fake.Core
open Fake.IO
open Fake.Core.TargetOperators

open BuildHelpers

initializeContext()

let sharedPath = Path.getFullName "src/Shared"
let serverPath = Path.getFullName "src/Server"
let clientPath = Path.getFullName "src/Client"
let clientOutputPath = Path.getFullName "src/Client/output"


// *****************
// * InstallClient *
// *****************
Target.create "InstallClient" (fun _ -> run yarn "" ".")


// *******
// * Run *
// *******
Target.create "Run" (fun _ ->
    [
        "server", dotnet "watch run" serverPath
        "client", dotnet $"fable watch -o {clientOutputPath} -s -e .jsx --run yarn run vite --host" clientPath
    ]
    |> runParallel
)


// ****************
// * Dependencies *
// ****************
"InstallClient"
    ==> "Run"
    |> ignore


[<EntryPoint>]
let main args = runOrDefault args



(*

Target.create "RunServer" (fun _ ->
    run dotnet "watch run" serverPath
)

Target.create "RunClient" (fun _ ->
    run dotnet "fable watch -o output -s --run yarn run dev" clientPath
)

// *********
// * Clean *
// *********
Target.create "Clean" (fun _ ->
    Shell.cleanDir deployPath
    run dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)



// **************
// * PkgAnalyze *
// **************
Target.create "PkgAnalyze" (fun _ ->
    run dotnet "femto" @"src\Client"
)


// **************
// * PkgResolve *
// **************
Target.create "PkgResolve" (fun _ ->
    run dotnet "femto --resolve" @"src\Client"
)



// ****************
// * Dependencies *
// ****************
"InstallClient"
    ==> "Run"
    |> ignore


[<EntryPoint>]
let main args = runOrDefault args
*)