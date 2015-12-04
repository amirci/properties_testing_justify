#r "packages/FAKE/tools/fakelib.dll"

open Fake.Testing
open Fake

[<AutoOpen>]
module Config =
    let testDir = "./"
    let srcDir  = "./"
    let prjName = "JustifyText"
    let mainSln = prjName + ".sln"
    let testPrj = prjName + ".Tests"
    let mainPrj = prjName

    let buildMode () = getBuildParamOrDefault "buildMode" "Release"
    let targetWithEnv target env = sprintf "%s:%s" target env

    let setParams defaults =
        { defaults with
            Targets = ["Build"]
            Properties =
                [
                    "Optimize", "True"
                    "Platform", "Any CPU"
                    "Configuration", buildMode()
                ]
        }

Target "Clean" (fun _ ->
  let clean config = {(setParams config) with Targets = ["Clean"]}
  build clean "JustifyText.sln"
)

Target "Build" (fun _ ->
  let rebuild config = {(setParams config) with Targets = ["Build"]}
  build rebuild "JustifyText.sln"
)

Target "Test" (fun _ ->
  !! (testDir @@ "/*.Tests.dll")
  |> xUnit (fun p -> {p with ToolPath = "packages/xunit.runners/tools/xunit.console.clr4.exe"})
)

"Build"
  ==> "Test"

RunTargetOrDefault "Build"
