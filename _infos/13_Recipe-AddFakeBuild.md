# Add a Fake build project

## Create project
```ps1
dotnet new console -n Build -o . -lang "F#"
# - rename Program.fs -> Build.fs
# - add <Import Project=".paket\Paket.Restore.targets" /> to project file

dotnet sln add Build.fsproj

dotnet paket add Fake.Core.Target --project Build.fsproj
# dotnet paket add Fake.IO.FileSystem --project Build.fsproj
# dotnet paket add Farmer --project Build.fsproj
```

## ./Build.fs
```f#
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
```

## ./BuildHelpers.fs
```f#
module BuildHelpers

open Fake.Core

let initializeContext () =
    let execContext = Context.FakeExecutionContext.Create false "build.fsx" [ ]
    Context.setExecutionContext (Context.RuntimeContext.Fake execContext)

module Proc =
    module Parallel =
        open System

        let locker = obj()

        let colors =
            [| ConsoleColor.Blue
               ConsoleColor.Yellow
               ConsoleColor.Magenta
               ConsoleColor.Cyan
               ConsoleColor.DarkBlue
               ConsoleColor.DarkYellow
               ConsoleColor.DarkMagenta
               ConsoleColor.DarkCyan |]

        let print color (colored: string) (line: string) =
            lock locker
                (fun () ->
                    let currentColor = Console.ForegroundColor
                    Console.ForegroundColor <- color
                    Console.Write colored
                    Console.ForegroundColor <- currentColor
                    Console.WriteLine line)

        let onStdout index name (line: string) =
            let color = colors.[index % colors.Length]
            if isNull line then
                print color $"{name}: --- END ---" ""
            else if String.isNotNullOrEmpty line then
                print color $"{name}: " line

        let onStderr name (line: string) =
            let color = ConsoleColor.Red
            if isNull line |> not then
                print color $"{name}: " line

        let redirect (index, (name, createProcess)) =
            createProcess
            |> CreateProcess.redirectOutputIfNotRedirected
            |> CreateProcess.withOutputEvents (onStdout index name) (onStderr name)

        let printStarting indexed =
            for (index, (name, c: CreateProcess<_>)) in indexed do
                let color = colors.[index % colors.Length]
                let wd =
                    c.WorkingDirectory
                    |> Option.defaultValue ""
                let exe = c.Command.Executable
                let args = c.Command.Arguments.ToStartInfo
                print color $"{name}: {wd}> {exe} {args}" ""

        let run cs =
            cs
            |> Seq.toArray
            |> Array.indexed
            |> fun x -> printStarting x; x
            |> Array.map redirect
            |> Array.Parallel.map Proc.run

let createProcess exe arg dir =
    CreateProcess.fromRawCommandLine exe arg
    |> CreateProcess.withWorkingDirectory dir
    |> CreateProcess.ensureExitCode

let dotnet = createProcess "dotnet"


let npm =
    let npmPath =
        match ProcessUtils.tryFindFileOnPath "npm" with
        | Some path -> path
        | None ->
            "npm was not found in path. Please install it and make sure it's available from your path. " +
            "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
            |> failwith
    createProcess npmPath


let yarn =
    let yarnPath =
        match ProcessUtils.tryFindFileOnPath "yarn" with
        | Some path -> path
        | None ->
            "yarn was not found in path. Please install it and make sure it's available from your path. "
            |> failwith
    createProcess yarnPath


let run proc arg dir =
    proc arg dir
    |> Proc.run
    |> ignore

let runParallel processes =
    processes
    |> Proc.Parallel.run
    |> ignore

let runOrDefault args =
    try
        match args with
        | [| target |] -> Target.runOrDefault target
        | _ -> Target.runOrDefault "Run"
        0
    with e ->
        printfn "%A" e
        1
```