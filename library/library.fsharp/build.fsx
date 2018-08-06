#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Paket
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open System.IO

Target.description "Cleanup Previous Build"
Target.create "Clean" (fun _ ->
    [|"bin"; "obj"|]
    |> Shell.cleanDirs
)

Target.description "Paket Package Restore"
Target.create "Restore" (fun _ ->
    if File.Exists("paket.lock")
        then Paket.restore (fun p -> { p with WorkingDir = "." })
        else Shell.Exec("mono", ".paket/paket.exe install")
             |> ignore
)

Target.description "Dotnet Build"
Target.create "Build" (fun _ ->
    !! "*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.description "Run All Targets"
Target.create "All" ignore

open Fake.Core.TargetOperators

"Clean"
    ==> "Restore"
    ==> "Build"
    ==> "All"

Target.runOrDefault "All"
