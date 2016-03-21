open System.IO
open System
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

let makeFileSystem (getFile: string -> byte[] option) = 
  let time = DateTime.Now
  let ni() = failwith "not implemented"
  { new IFileSystem with
    member x.AssemblyLoad n = ni()
    member x.AssemblyLoadFrom n = ni()
    member x.FileDelete n = ni()
    member x.FileStreamCreateShim n = ni()
             
    member x.FileStreamReadShim fileName = 
      match getFile fileName with
      | Some contents -> new MemoryStream( contents ) :> Stream
      | None -> null
             
    member x.FileStreamWriteExistingShim n = ni()
    member x.GetFullPathShim fileName = fileName
    member x.GetLastWriteTimeShim n = time
    member x.GetTempPathShim() = System.IO.Path.GetTempPath()
    member x.IsInvalidPathShim n = false
    member x.IsPathRootedShim n = false

    member x.ReadAllBytesShim(fileName: string): byte [] = 
      match getFile fileName with
      | Some contents -> contents
      | None -> 
        try
          File.ReadAllBytes fileName
        with _ -> null
             
    member x.SafeExists fileName = 
      (getFile fileName |> Option.isSome) || (File.Exists fileName)
  }
