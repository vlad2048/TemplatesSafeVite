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

Target.create "RunServer" (fun _ ->
    run dotnet "watch run" serverPath
)

Target.create "RunClient" (fun _ ->
    run dotnet $"fable watch -o {clientOutputPath} -s -e .jsx --run yarn run vite --host" clientPath
)


// ****************
// * Dependencies *
// ****************
"InstallClient"
    ==> "Run"
    |> ignore


[<EntryPoint>]
let main args = runOrDefault args
