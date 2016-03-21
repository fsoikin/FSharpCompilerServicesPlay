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

let srcBytes = 
  """
      module X
    
      let f x y = x+y
      let g = f 1
      let h = (g 2) + 3
  """ 
  |> System.Text.Encoding.UTF8.GetBytes

Shim.FileSystem <- makeFileSystem (fun fn -> if fn = "file.fs" then Some srcBytes else None)

let res = checker.ParseAndCheckProject( opts ) |> Async.RunSynchronously
let ast = res.AssemblyContents.ImplementationFiles

(*
val ast : FSharpImplementationFileContents list =
  [Microsoft.FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents
     {Declarations = [Entity
                        (X,

      [MemberOrFunctionOrValue
        (val f,[[val x]; [val y]],
        Call
          (null,val op_Addition,[],
           [type Microsoft.FSharp.Core.int; type Microsoft.FSharp.Core.int;
            type Microsoft.FSharp.Core.int],[Value val x; Value val y]));

       MemberOrFunctionOrValue
        (val g,[],
        Let
          ((val x, Const (1,type Microsoft.FSharp.Core.int)),
           Lambda (val y,Call (null,val f,[],[],[Value val x; Value val y]))));

       MemberOrFunctionOrValue
        (val h,[],
        Application
          (Call (null,val g,[],[],[]),[],[Const (2,type Microsoft.FSharp.Core.int)]))
        // ^^^^^ no call to (+) and (Const 3) is missing!
      ])];

      FileName = "file.fs";
      HasExplicitEntryPoint = false;
      IsScript = false;
      QualifiedName = "X";}]
*)