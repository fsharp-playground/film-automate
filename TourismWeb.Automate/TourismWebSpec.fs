namespace TourismWeb.Automate

module TourismWebSpec =


    #if INTERACTIVE
    #I "bin/Debug"
    #r "canopy.dll"
    #r "WebDriver.dll"
    #r "NLog.dll"
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
    open NLog
    open FSharp.Data

    System.IO.Directory.SetCurrentDirectory(path)

    type Logger with
        member this.Json obj = 
            JsonConvert.SerializeObject obj |> this.Trace

    type Property = FSharp.Data.JsonProvider<"Property.json">

    let Test (case:string) = (case.StartsWith "_" = false, case)

    let Config url =
        configuration.chromeDir <- path
        configuration.elementTimeout <- 5.0
        configuration.compareTimeout <- 3.0
        configuration.pageTimeout <- 3.0
        configuration.runFailedContextsFirst <- true
        configuration.reporter <- new LiveHtmlReporter(Chrome, configuration.chromeDir) :> IReporter
        configuration.failFast := true
        runner.context ("Tourism Web @ " + url)
//        runner.once (fun _ -> Console.WriteLine "once")
//        runner.before(fun _ -> Console.WriteLine "before")
//        runner.after(fun _ -> Console.WriteLine "alfter")
//        runner.lastly(fun _ -> Console.WriteLine "lastly")

    let Start() = core.start core.chrome

    let Run() = runner.run(); //core.quit()

    let Str (obj:obj) = obj.ToString()

    let NgModel str = sprintf """[ng-model="%s"]""" str

    let KngModel str = sprintf """[k-ng-model="%s"]""" str

    let Option value = read <| sprintf """option[value="%s"]""" value


    let LetsItGo() =

        let jw = File.ReadAllText("Property.json") |> Property.Parse
        let logger = LogManager.GetCurrentClassLogger()

        Config jw.TestUrl
        Start()

        match jw.C1.Title |> Test with
        | (true, case) -> 
            case &&& fun _ ->
                core.url jw.TestUrl
                "[name=userName]"  << jw.C1.UserName 
                "[name=password]" << jw.C1.Password.ToString()
                click "[type=submit]"
                "div.tr-logout > a" == "ออกจากระบบ"
        | _ -> ()

        match jw.C2.Title |> Test with
        | (true, case) -> 
            case &&& fun _ ->

                click "[ui-sref=request]"
                click "โฆษณา"

                (* kendo ui dropdown list is a combinationof a tree of spans
                and a completely decoupled hidden div containing an unordered list
                which is dynamically positioned using position:absolute
                *)
                //let el = NgModel "selectedCountryId" |> element 
                //el << "5"

                NgModel "st.applicant.name" << jw.C2.ApplicantName
                NgModel "st.applicant.address" << Str jw.C2.ApplicantAddress
                NgModel "st.applicant.postcode" << Str jw.C2.ApplicantPostcode
                //NgModel "selectedCountryId" << jw.C2.ApplicantCountry
                NgModel "st.applicant.telephone" << Str jw.C2.ApplicantTelephone
                NgModel "st.applicant.fax" << Str jw.C2.ApplicantFax
                NgModel "st.applicant.email" << jw.C2.ApplicantEmail
                NgModel "st.applicant.website" << jw.C2.ApplicantWebsite

                click  "#i2"
                
                NgModel "st.film.budget" << Str jw.C2.FilmBudget
                //NgModel "st.film.selectedFormatId" << Str jw.C2.FilmSelectedFormatId
                KngModel "st.film.startFilming" << jw.C2.FilmStartFilming.ToString "MM/dd/yyyy"
                KngModel "st.film.endFilming" << jw.C2.FilmEndFilming.ToString "MM/dd/yyyy"
                NgModel "st.film.lengthInHour" << Str jw.C2.FilmLengthInHour
                NgModel "st.film.lengthInMinute" << Str jw.C2.FilmLengthInMinute
                NgModel "st.film.lengthInSecond" << Str jw.C2.FilmLengthInSecond
                click """[ng-click="vm.update()"]"""

                (element """//div[contains(text(), "บันทึกข้อมูลเรียบร้อย")""").Displayed === true

        | _ ->()

        Run()


