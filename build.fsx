#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target 
nuget Fake.JavaScript.Yarn //"
#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.JavaScript

let slnPath = "./elmish-firebase-research.sln"
let appDirectory = "./src/SampleApp"

let inline yarnWorkDir (ws : string) (yarnParams : Yarn.YarnParams) =
  { yarnParams with WorkingDirectory = ws }

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
  !! "src/**/bin"
  ++ "src/**/obj"
  ++ "dist"
  |> Shell.cleanDirs 
)

Target.create "YarnInstall" (fun _ ->
  Yarn.install id
)

Target.create "Webpack" (fun _ ->
  Yarn.exec "webpack" (yarnWorkDir appDirectory)
)

Target.create "WebPackDevServer" (fun _ ->
  Yarn.exec "webpack serve" (yarnWorkDir appDirectory)
)

Target.create "DotNetRestore" (fun _ ->
  DotNet.restore id slnPath
)

Target.create "Setup" ignore

"Clean"
  ==> "YarnInstall"
  ==> "DotNetRestore"
  ==> "Setup"

"Setup"
  ==> "Webpack"

"Setup"
  ==> "WebpackDevServer"

Target.runOrDefault "Setup"
