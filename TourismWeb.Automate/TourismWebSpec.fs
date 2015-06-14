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

    do
        #if INTERACTIVE
        let path = __SOURCE_DIRECTORY__ 
        #else
        let path = "./"
        #endif
        System.IO.Directory.SetCurrentDirectory(path)

    type Logger with
        member this.Json obj = 
            JsonConvert.SerializeObject obj |> this.Trace

    type Property = FSharp.Data.JsonProvider<"Property.json">

    let Test (case:string) = (case.StartsWith "_" = false, case)

    let Config url =
        configuration.chromeDir <- "./"
        configuration.elementTimeout <- 77.0
        configuration.compareTimeout <- 3.0
        configuration.pageTimeout <- 3.0
        configuration.runFailedContextsFirst <- true
        //configuration.reporter <- new LiveHtmlReporter(Chrome, configuration.chromeDir) :> IReporter
        configuration.failFast := true
        runner.context ("Tourism Web @ " + url)

    let Start() = core.start core.chrome

    let Run() = runner.run(); //core.quit()

    let Str (obj:obj) = obj.ToString()

    let Date (obj:DateTime) = obj.ToString("MM/dd/yyyy")

    let Time (obj:DateTime) = obj.ToString("hh:mm tt")

    let NgModel str = sprintf """[ng-model="%s"]""" str

    let KngModel str = sprintf """[k-ng-model="%s"]""" str

    let Option value = read <| sprintf """option[value="%s"]""" value

    let jw = File.ReadAllText("Property.json") |> Property.Parse
    let logger = LogManager.GetCurrentClassLogger()

    let LoginSpec() =
        let login = jw.Login;
        match login.Title |> Test with
        | (true, case) -> 
            case &&& fun _ ->
                core.url jw.TestUrl
                "[name=userName]"  << login.UserName 
                "[name=password]" << login.Password.ToString()
                click "[type=submit]"
                "div.tr-logout > a" == "ออกจากระบบ"
        | _ -> ()

    let RequestStep1Spec() =
        let r1 = jw.R;
        match r1.Title |> Test with
        | (true, case) -> 
            case &&& fun _ ->

                click "[ui-sref=request]"
                click "โฆษณา"

                (* kendo ui dropdown list is a combinationof a tree of spans
                and a completely decoupled hidden div containing an unordered list
                which is dynamically positioned using position:absolute
                let el = NgModel "selectedCountryId" |> element *)

                NgModel "st.applicant.name" << r1.Applicant.Name
                NgModel "st.applicant.address" << Str r1.Applicant.Address
                NgModel "st.applicant.postcode" << Str r1.Applicant.Postcode
                //NgModel "selectedCountryId" << r1.ApplicantCountry
                NgModel "st.applicant.telephone" << Str r1.Applicant.Telephone
                NgModel "st.applicant.fax" << Str r1.Applicant.Fax
                NgModel "st.applicant.email" << r1.Applicant.Email
                NgModel "st.applicant.website" << r1.Applicant.Website

                "#i2" |> click
                
                NgModel "st.film.title" << r1.Film.Title
                NgModel "st.film.budget" << Str r1.Film.Budget
                //NgModel "st.film.selectedFormatId" << Str r1.FilmSelectedFormatId
                KngModel "st.film.startFilming" << Date r1.Film.StartFilming
                KngModel "st.film.endFilming" << Date r1.Film.EndFilming
                NgModel "st.film.lengthInHour" << Str r1.Film.LengthInHour
                NgModel "st.film.lengthInMinute" << Str r1.Film.LengthInMinute
                NgModel "st.film.lengthInSecond" << Str r1.Film.LengthInSecond
                """[ng-click="vm.update()"]""" |> click

                """button.confirm""" |> click

        | _ ->()

    let RequestStep2Spec() =
        let r2 = jw.R2
        match r2.Title |> Test with 
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]""" |> click
                NgModel "st.staff.firstName" << r2.Staff.FirstName
                NgModel "st.staff.middleName" << r2.Staff.MiddleName
                NgModel "st.staff.lastName" << r2.Staff.LastName
                NgModel "st.staff.age" << Str r2.Staff.Age
                NgModel "st.staff.passportNo" << Str r2.Staff.PassportNo
                KngModel "st.staff.expireDate" << Date r2.Staff.ExpireDate
                KngModel "st.staff.stayFrom" << Date r2.Staff.StayFrom
                KngModel "st.staff.stayTo" << Date r2.Staff.StayTo

                """[ng-click="vm.addStaff()"]""" |> click
                "li.k-last" |> click

                KngModel "st.team.arriveDate" << Date r2.Team.ArriveDate
                KngModel "st.team.workingFrom" << Date r2.Team.WorkingFrom
                KngModel "st.team.workingTo" << Date r2.Team.WorkingTo

                NgModel "st.teamAddress.accommodation" << r2.TeamAddress.Accommodation
                NgModel "st.teamAddress.address" << r2.TeamAddress.Address
                NgModel "st.teamAddress.telephone" << Str r2.TeamAddress.Telephone
                NgModel "st.teamAddress.fax" << Str r2.TeamAddress.Fax
                NgModel "st.teamAddress.email" << r2.TeamAddress.Email

                """[ng-click="vm.update()"]""" |> click
                """button.confirm""" |> click

        | _ -> ()


    let RequestStep3Spec() =
        let r3 = jw.R3
        match r3.Title |> Test with
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]""" |> click
                "st.filmingLocation.name" |> NgModel << r3.FilmingLocation.Name
                "st.filmingLocation.address" |> NgModel << r3.FilmingLocation.Address
                "st.filmingLocation.startDate" |> KngModel << Date r3.FilmingLocation.StartDate
                "st.filmingLocation.endDate" |> KngModel << Date r3.FilmingLocation.EndDate
                "st.filmingLocation.startTime" |> KngModel << Time r3.FilmingLocation.StartTime
                "st.filmingLocation.endTime" |> KngModel << Time r3.FilmingLocation.EndTime
                "st.filmingLocation.scene" |> NgModel << r3.FilmingLocation.Scene
                "st.filmingLocation.detail" |> NgModel << r3.FilmingLocation.Detail
                "st.filmingLocation.latitude" |> NgModel << Str r3.FilmingLocation.Latitude
                "st.filmingLocation.longitude" |> NgModel << Str r3.FilmingLocation.Longitude

                """[ng-click="vm.addLocation()"]""" |> click
                """[ng-click="vm.update()"]""" |> click
                """button.confirm""" |> click

        | _ -> ()


    let RequestStep4Spec() =
        let r4 = jw.R4;
        match r4.Title |> Test with
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]""" |> click
                let els = """[ng-click="vm.generate(form)"]""" |> core.elements
                els |> List.iter (fun el -> el |> click)

                //let views = """.file.pdf.outline""" |> core.elements
                //views.Count() === els.Count()
                ".confirm" |> notDisplayed

        | _ -> ()

    let LetsItGo() =
        Config jw.TestUrl
        Start()

        LoginSpec()
        RequestStep1Spec()
        RequestStep2Spec()
        RequestStep3Spec()
        RequestStep4Spec()

        Run()
