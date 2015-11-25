#r "System.Data.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"

open System
open System.Data
open System.Text
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

type schema = SqlDataConnection< @"data source=DIXON13J\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=NerdDinner;MultipleActiveResultSets=True">
let context = schema.GetDataContext()

let getRandomDay(month:int) =
    let random = new Random()
    match month with
        | 1 | 3 | 5 | 7 | 8 | 10 | 12 -> random.Next(30) + 1
        | 4 | 6 | 9 | 11 -> random.Next(29) + 1
        | 2 -> random.Next(27) + 1
        | _ -> 1

let getRandomDate() =
    let random = new Random()
    let year = 2014
    let month = random.Next(11) + 1
    let day = getRandomDay(month)
    new DateTime(year,month,day,18,0,0)

let hosts = [|"scottgu";"scottha";"philha";"robcon";"billg";"James"|]

let getRandomHost() =
    let random = new Random()
    let index = random.Next(5)
    hosts.[index]    

let getRandomRSVP(dinnerId) =
    let random = new Random()
    let RSVP = new schema.ServiceTypes.RSVPs()
    RSVP.AttendeeName <- getRandomHost()
    RSVP.AttendeeNameId <- RSVP.AttendeeName
    RSVP.DinnerID <- dinnerId
    RSVP

let createRandomDinner(dinnerIndex) =
    let random = new Random()
    let dinner = new schema.ServiceTypes.Dinners()
    dinner.Address <- "123 Main Street"
    dinner.ContactPhone <- "919-555-1212"
    dinner.Country <- "USA"
    dinner.Description <- "Random Dinner #" + random.Next(99).ToString()
    dinner.EventDate <- getRandomDate()
    dinner.HostedBy <- getRandomHost()
    dinner.HostedById <- dinner.HostedBy
    dinner.Latitude <- 47.64312
    dinner.Longitude <- -122.130609
    dinner.Title <- dinner.Description
    dinner

let insertDinner() =
    let dinner = createRandomDinner()
    context.Dinners.InsertOnSubmit(dinner)
    context.DataContext.SubmitChanges()
    dinner.DinnerID

let insertRSVP(dinnerId) =
    let rsvp = getRandomRSVP(dinnerId)
    context.RSVPs.InsertOnSubmit(rsvp)
    ()

let insertRSVPs(dinnerId) =
    let random = new Random()
    let rsvpLength = random.Next(9) + 3
    let rsvpCount = Array.init rsvpLength (fun index -> index)
    rsvpCount |> Array.iter(fun _ -> insertRSVP(dinnerId))
    context.DataContext.SubmitChanges()
    ()

let createEntry() =
    let dinnerId = insertDinner()
    insertRSVPs(dinnerId)

let entryCount = Array.init 100 (fun index -> index)
entryCount |> Array.iter(fun _ -> createEntry())
