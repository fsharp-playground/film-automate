#I @"..\packages\FSharp.Data.2.2.2\lib\net40"
#r "FSharp.Data.dll"

open System.IO

do 
    let path = __SOURCE_DIRECTORY__ 
    Directory.SetCurrentDirectory(path)

type Property = FSharp.Data.JsonProvider< "Info.json" >

let info = File.ReadAllText("Info.json") |> Property.Parse

let n1 = info.A.Title
let n2 = info.B.Policeliasion.FirstName