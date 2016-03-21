#load "compiler.fsx"

let error = Compiler.getTast
                """
                  module X
    
                  let f x y = x+y
                  let g = f 1
                  let h = (g 2) + 3
                """ 

(*
val error : FSharpImplementationFileDeclaration list =
   [Entity (X,

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
      ])
*)

let stillError = Compiler.getTast
                    """
                      module X
    
                      let f x y = x+y
                      let g = f 1
                      let h = (g 2) + 3

                      printfn "%s" h
                      let z = ()
                    """ 

let correct1 = Compiler.getTast
                """
                  module X
    
                  let f x y = x+y
                  let g = 2
                  let h = g + 3
                """ 

let correct2 = Compiler.getTast
                """
                  module X
    
                  let f x y = x+y
                  let g x = x + 2
                  let h = (g 5) + 3
                """ 
