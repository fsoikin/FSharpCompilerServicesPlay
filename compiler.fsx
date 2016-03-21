#r "./packages/FSharp.Compiler.Service/lib/net45/FSharp.Compiler.Service.dll"
#load "fileSystem.fsx"

open System
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.SourceCodeServices
open FileSystem

let checker = FSharpChecker.Create(keepAssemblyContents=true)
let opts = {
  FSharpProjectOptions.IsIncompleteTypeCheckEnvironment = false
  ProjectFileName = "project.fsproj"
  ProjectFileNames = [|"file.fs"|]
  OtherOptions = [|"--out:x.dll"; "--target:library"|]
  ReferencedProjects = [||]
  UseScriptResolutionRules = false
  LoadTime = DateTime.Now
  UnresolvedReferences = None }

let getTast srcCode =
  let srcBytes = System.Text.Encoding.UTF8.GetBytes( srcCode: string )
  Shim.FileSystem <- makeFileSystem (fun fn -> if fn = "file.fs" then Some srcBytes else None)

  let res = checker.ParseAndCheckProject( opts ) |> Async.RunSynchronously
  let ast = res.AssemblyContents.ImplementationFiles.[0].Declarations

  ast