
#if INTERACTIVE
#I "bin/Debug"
#r "canopy.dll"
#r "WebDriver.dll"
#r "NLog.dll"
#r "FSharp.Data.dll"
#r "Newtonsoft.Json.dll"
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
open NLog
open FSharp.Data
open OpenQA.Selenium.Remote

let config() =
    configuration.chromeDir <- "./"
    configuration.elementTimeout <- 10.0
    configuration.compareTimeout <- 10.0
    configuration.pageTimeout <- 10.0
    configuration.runFailedContextsFirst <- true
    configuration.failFast := true


let json obj =
    let js = JsonConvert.SerializeObject(obj,Formatting.Indented)
    js |> Console.WriteLine

let start() = 
    let caps =  DesiredCapabilities.Chrome() :> ICapabilities
    let url = "http://10.0.0.146:4444/wd/hub"
    core.start (Remote (url, caps))

(* must exist *)
let file() = "/Users/wk/Resource/test/gov.pdf"

let attach() =
    url "http://10.211.55.3:3000"
    let els = """[type="file"]""" |> core.elements 
    els |> List.iter(fun el -> 
        file() |> el.SendKeys 
        file() |> Console.WriteLine)


    let el = """[type="file"]""" |> core.element
    let dsp = el.Displayed
    file() |> el.SendKeys

let go() =
    config()
    start()


