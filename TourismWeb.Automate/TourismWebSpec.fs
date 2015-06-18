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
    open OpenQA.Selenium.Remote

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

    let jw = File.ReadAllText("Property.json") |> Property.Parse
    let logger = LogManager.GetCurrentClassLogger()

    let test (case:string) = (case.StartsWith "_" = false, case)

    let config url =
        configuration.chromeDir <- "./"
        configuration.elementTimeout <- 20.0
        configuration.compareTimeout <- 20.0
        configuration.pageTimeout <- 20.0
        configuration.runFailedContextsFirst <- true
        //configuration.reporter <- new LiveHtmlReporter(Chrome, configuration.chromeDir) :> IReporter
        configuration.failFast := true
        runner.context ("Tourism Web @ " + url)

    let start() = 
        let driver = (jw.GridUrl, DesiredCapabilities.Chrome() :> ICapabilities)
        core.start (Remote driver)
        core.pin FullScreen

    let run() = runner.run(); //core.quit()

    let str (obj:obj) = obj.ToString()

    let date (obj:DateTime) = obj.ToString("MM/dd/yyyy")

    let time (obj:DateTime) = obj.ToString("hh:mm tt")

    let ngModel str = sprintf """[ng-model="%s"]""" str

    let kngModel str = sprintf """[k-ng-model="%s"]""" str

    let Option value = read <| sprintf """option[value="%s"]""" value

    let getFile() = jw.AttachFile

    let dropdown value name = 
        let cmd = sprintf """$('%s').data("kendoDropDownList").select(%d)""" name value
        Console.WriteLine(cmd)
        cmd |> js |> ignore

    let loginSpec() =
        let login = jw.Login;
        match login.Title |> test with
        | (true, case) -> 
            case &&& fun _ ->
                core.url jw.TestUrl
                "[name=userName]"  << login.UserName 
                "[name=password]" << login.Password.ToString()
                click "[type=submit]"
                "div.tr-logout > a" == "ออกจากระบบ"
        | _ -> ()

    let requestStep1Spec() =
        let r1 = jw.S
        match r1.Title |> test with
        | (true, case) -> 
            case &&& fun _ ->

                "[ui-sref=request]"         |> click
                "โฆษณา"                     |> click

                "st.applicant.name"         |> ngModel << r1.Applicant.Name
                "st.applicant.address"      |> ngModel << str r1.Applicant.Address
                "st.applicant.postcode"     |> ngModel << str r1.Applicant.Postcode
                "selectedCountryId"         |> ngModel |> dropdown r1.Applicant.CountryDn
                "st.applicant.telephone"    |> ngModel  << str r1.Applicant.Telephone
                "st.applicant.fax"          |> ngModel  << str r1.Applicant.Fax
                "st.applicant.email"        |> ngModel  << r1.Applicant.Email
                "st.applicant.website"      |> ngModel  << r1.Applicant.Website

                //"#i2"                       |> click
                
                "st.film.title"             |> ngModel << r1.Film.Title
                "st.film.budget"            |> ngModel << str r1.Film.Budget
                "selectedFormatId"          |> ngModel |> dropdown r1.Film.FormatDn
                "st.film.startFilming"      |> kngModel << date r1.Film.StartFilming
                "st.film.endFilming"        |> kngModel << date r1.Film.EndFilming
                "st.film.lengthInHour"      |> ngModel  << str r1.Film.LengthInHour
                "st.film.lengthInMinute"    |> ngModel << str r1.Film.LengthInMinute
                "st.film.lengthInSecond"    |> ngModel << str r1.Film.LengthInSecond

                """[ng-click="vm.update()"]""" |> click
                """button.confirm"""        |> click

        | _ ->()

    let requestStep2Spec() =
        let r2 = jw.S2
        match r2.Title |> test with 
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]""" |> click

                "st.staff.firstName"    |> ngModel << r2.Staff.FirstName
                "st.staff.middleName"   |> ngModel << r2.Staff.MiddleName
                "st.staff.lastName"     |> ngModel << r2.Staff.LastName
                "st.staff.age"          |> ngModel << str r2.Staff.Age
                "st.staff.passportNo"   |> ngModel << str r2.Staff.PassportNo
                "st.staff.expireDate"   |> kngModel << date r2.Staff.ExpireDate
                "st.staff.stayFrom"     |> kngModel << date r2.Staff.StayFrom
                "st.staff.stayTo"       |> kngModel << date r2.Staff.StayTo

                "selectedNationalityId" |> ngModel  |> dropdown r2.Staff.NationalityDn 
                "selectedEthnicityId"   |> ngModel  |> dropdown r2.Staff.EthnicityDn
                "selectedPositionId"    |> ngModel  |> dropdown r2.Staff.PositionDn
                "selectedTitleId"       |> ngModel  |> dropdown r2.Staff.TitleDn


                """[ng-click="vm.addStaff()"]""" |> click
                //"li.k-last" |> click

                "st.team.arriveDate"   |> kngModel << date r2.Team.ArriveDate
                "st.team.workingFrom"  |> kngModel << date r2.Team.WorkingFrom
                "st.team.workingTo"    |> kngModel << date r2.Team.WorkingTo

                "st.teamAddress.accommodation"  |> ngModel << r2.TeamAddress.Accommodation
                "st.teamAddress.address"        |> ngModel << r2.TeamAddress.Address
                "st.teamAddress.telephone"      |> ngModel << str r2.TeamAddress.Telephone
                "st.teamAddress.fax"            |> ngModel << str r2.TeamAddress.Fax
                "st.teamAddress.email"          |> ngModel << r2.TeamAddress.Email

                "selectedProvinceId"    |> ngModel |> dropdown r2.TeamAddress.ProvinceDn

                """[ng-click="vm.update()"]""" |> click
                """button.confirm""" |> click

        | _ -> ()


    let requestStep3Spec() =
        let r3 = jw.S3
        match r3.Title |> test with
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]"""    |> click

                "st.filmingLocation.name"       |> ngModel << r3.FilmingLocation.Name
                "st.filmingLocation.address"    |> ngModel << r3.FilmingLocation.Address
                "st.filmingLocation.startDate"  |> kngModel << date r3.FilmingLocation.StartDate
                "st.filmingLocation.endDate"    |> kngModel << date r3.FilmingLocation.EndDate
                "st.filmingLocation.startTime"  |> kngModel << time r3.FilmingLocation.StartTime
                "st.filmingLocation.endTime"    |> kngModel << time r3.FilmingLocation.EndTime
                "st.filmingLocation.scene"      |> ngModel << r3.FilmingLocation.Scene
                "st.filmingLocation.detail"     |> ngModel << r3.FilmingLocation.Detail
                "st.filmingLocation.latitude"   |> ngModel << str r3.FilmingLocation.Latitude
                "st.filmingLocation.longitude"  |> ngModel << str r3.FilmingLocation.Longitude

                "selectedProvinceId"    |> ngModel |> dropdown r3.FilmingLocation.ProvinceDn

                """[ng-click="vm.addLocation()"]""" |> click
                """[ng-click="vm.update()"]""" |> click
                """button.confirm""" |> click

        | _ -> ()


    let requestStep4Spec() =
        let r4 = jw.S4
        match r4.Title |> test with
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]""" |> click
                let els = """[ng-click="vm.generate(form)"]""" |> core.elements
                els |> List.iter (fun el -> el |> click)

                let finish() = 
                    let views = """.file.pdf.outline""" |> core.elements
                    sprintf "%d <> %d" (views.Count()) (els.Count()) |> core.describe
                    views.Count() = els.Count()

                core.waitFor finish

        | _ -> ()

    let requestStep5Spec() =
        let r5 = jw.S5
        match r5.Title |> test with
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]""" |> click
                let els = """[type="file"]""" |> core.elements 

                els |> List.iter(fun el -> 
                    getFile() |> el.SendKeys 
                    getFile() |> Console.WriteLine)

                let wait() = 
                    let views = """.file.pdf.outline""" |> core.elements
                    sprintf "%d <> %d" (views.Count()) (els.Count()) |> core.describe
                    views.Count() = els.Count()

                core.waitFor wait

                //"li.k-last" |> click

                "st.policeLiasion.firstName"    |> ngModel << r5.Pl.FirstName
                "st.policeLiasion.email"        |> ngModel << r5.Pl.Email 
                "st.policeLiasion.telephone"    |> ngModel << str r5.Pl.Telephone

                """[ng-click="vm.update()"]"""  |> click
                ".confirm" |> click

        | _ -> ()

    let requestStep6Spec() =
        let r6 = jw.S6
        match r6.Title |> test with
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]"""    |> click
                """[ng-click="vm.start()"]"""   |> click
                ".confirm"                      |> click
        | _ -> ()

    let _jw() =
        let els = sprintf """$('[type="file"]').val('%s')"""  (getFile())
        let rs = els |> js

        let el = element """[type="file"]"""
        el << getFile()
        ()

    let letsItGo() =

        config jw.TestUrl
        start()
        loginSpec()

        requestStep1Spec()
        requestStep2Spec()
        requestStep3Spec()
        requestStep4Spec()
        requestStep5Spec()
        requestStep6Spec()

        run()
