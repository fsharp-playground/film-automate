

#if INTERACTIVE
#I @"..\packages\FSharp.Data.2.2.2\lib\net40"
#I @"..\packages\canopy.0.9.22\lib"
#I @"..\packages\Selenium.WebDriver.2.45.0\lib\net40"
#r "canopy.dll"
#r "WebDriver.dll"
#r "FSharp.Data.dll"
#r "Newtonsoft.Json.dll"
#endif

#if INTERACTIVE
let path = __SOURCE_DIRECTORY__ 
#else
let path = "./"
#endif

open System
open OpenQA.Selenium
open canopy.configuration
open canopy.runner
open canopy.reporters
open canopy.types
open canopy
open System.IO
open System.Linq
open Newtonsoft.Json
open FSharp.Data


type Property = FSharp.Data.JsonProvider<"Property.json">

