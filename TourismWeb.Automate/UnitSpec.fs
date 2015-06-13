namespace Automate

// these are similar to C# using statements
#if INTERACTIVE
#I "bin\Debug"
#r "canopy"
#endif

open canopy
open runner
open System

open NUnit.Framework

module UnitSpec = 

    let TestDir() =
        let dir = System.IO.Directory.GetCurrentDirectory()
        System.IO.Directory.SetCurrentDirectory @"E:\source\projects\tourism\entity\TourismEntity\TourismWeb.Automate"
        ()

    let Login() =
        canopy.configuration.chromeDir <- @".\"
        start chrome
        url "http://10.0.0.200:9991/"
        "[name=userName]" << "wk"
        "[name=password]" << "1234"
        click "[type=submit]"

    [<Test>]
    let ``Should ((Login)) as ((WK))``() =  
        Login()
        """[href="/default/logout"]""" == "ออกจากระบบ"

    [<Test>]
    let ``Should ((Navigate)) to ((Search Page))``() =
        click """[ui-sref="m1"]"""
        click """[ui-sref="m1.search"]"""
