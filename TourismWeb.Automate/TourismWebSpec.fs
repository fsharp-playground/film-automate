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

    let DropDown value name = 
        let cmd = sprintf """$('%s').data("kendoDropDownList").select(%d)""" name value
        Console.WriteLine(cmd)
        cmd |> js |> ignore

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

                "[ui-sref=request]"         |> click
                "โฆษณา"                     |> click

                "st.applicant.name"         |> NgModel << r1.Applicant.Name
                "st.applicant.address"      |> NgModel << Str r1.Applicant.Address
                "st.applicant.postcode"     |> NgModel << Str r1.Applicant.Postcode
                "selectedCountryId"         |> NgModel |> DropDown r1.Applicant.CountryDn
                "st.applicant.telephone"    |> NgModel  << Str r1.Applicant.Telephone
                "st.applicant.fax"          |> NgModel  << Str r1.Applicant.Fax
                "st.applicant.email"        |> NgModel  << r1.Applicant.Email
                "st.applicant.website"      |> NgModel  << r1.Applicant.Website

                "#i2"                       |> click
                
                "st.film.title"             |> NgModel << r1.Film.Title
                "st.film.budget"            |> NgModel << Str r1.Film.Budget
                "selectedFormatId"          |> NgModel |> DropDown r1.Film.FormatDn
                "st.film.startFilming"      |> KngModel << Date r1.Film.StartFilming
                "st.film.endFilming"        |> KngModel << Date r1.Film.EndFilming
                "st.film.lengthInHour"      |> NgModel  << Str r1.Film.LengthInHour
                "st.film.lengthInMinute"    |> NgModel << Str r1.Film.LengthInMinute
                "st.film.lengthInSecond"    |> NgModel << Str r1.Film.LengthInSecond

                """[ng-click="vm.update()"]""" |> click
                """button.confirm"""        |> click

        | _ ->()

    let RequestStep2Spec() =
        let r2 = jw.R2
        match r2.Title |> Test with 
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]""" |> click

                "st.staff.firstName"    |> NgModel << r2.Staff.FirstName
                "st.staff.middleName"   |> NgModel << r2.Staff.MiddleName
                "st.staff.lastName"     |> NgModel << r2.Staff.LastName
                "st.staff.age"          |> NgModel << Str r2.Staff.Age
                "st.staff.passportNo"   |> NgModel << Str r2.Staff.PassportNo
                "st.staff.expireDate"   |> KngModel << Date r2.Staff.ExpireDate
                "st.staff.stayFrom"     |> KngModel << Date r2.Staff.StayFrom
                "st.staff.stayTo"       |> KngModel << Date r2.Staff.StayTo

                "selectedNationalityId" |> NgModel  |> DropDown r2.Staff.NationalityDn 
                "selectedEthnicityId"   |> NgModel  |> DropDown r2.Staff.EthnicityDn
                "selectedPositionId"    |> NgModel  |> DropDown r2.Staff.PositionDn
                "selectedTitleId"       |> NgModel  |> DropDown r2.Staff.TitleDn


                """[ng-click="vm.addStaff()"]""" |> click
                "li.k-last" |> click

                "st.team.arriveDate"   |> KngModel << Date r2.Team.ArriveDate
                "st.team.workingFrom"  |> KngModel << Date r2.Team.WorkingFrom
                "st.team.workingTo"    |> KngModel << Date r2.Team.WorkingTo

                "st.teamAddress.accommodation"  |> NgModel << r2.TeamAddress.Accommodation
                "st.teamAddress.address"        |> NgModel << r2.TeamAddress.Address
                "st.teamAddress.telephone"      |> NgModel << Str r2.TeamAddress.Telephone
                "st.teamAddress.fax"            |> NgModel << Str r2.TeamAddress.Fax
                "st.teamAddress.email"          |> NgModel << r2.TeamAddress.Email

                "selectedProvinceId"    |> NgModel |> DropDown r2.TeamAddress.ProvinceDn

                """[ng-click="vm.update()"]""" |> click
                """button.confirm""" |> click

        | _ -> ()


    let RequestStep3Spec() =
        let r3 = jw.R3
        match r3.Title |> Test with
        | (true, case) ->
            case &&& fun _ ->
                """[ng-click="vm.next()"]"""    |> click

                "st.filmingLocation.name"       |> NgModel << r3.FilmingLocation.Name
                "st.filmingLocation.address"    |> NgModel << r3.FilmingLocation.Address
                "st.filmingLocation.startDate"  |> KngModel << Date r3.FilmingLocation.StartDate
                "st.filmingLocation.endDate"    |> KngModel << Date r3.FilmingLocation.EndDate
                "st.filmingLocation.startTime"  |> KngModel << Time r3.FilmingLocation.StartTime
                "st.filmingLocation.endTime"    |> KngModel << Time r3.FilmingLocation.EndTime
                "st.filmingLocation.scene"      |> NgModel << r3.FilmingLocation.Scene
                "st.filmingLocation.detail"     |> NgModel << r3.FilmingLocation.Detail
                "st.filmingLocation.latitude"   |> NgModel << Str r3.FilmingLocation.Latitude
                "st.filmingLocation.longitude"  |> NgModel << Str r3.FilmingLocation.Longitude

                "selectedProvinceId"    |> NgModel |> DropDown r3.FilmingLocation.ProvinceDn

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
