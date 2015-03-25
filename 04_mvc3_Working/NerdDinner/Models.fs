namespace NerdDinner.Models

open System
open FSharp.Data
open System.Web.Mvc
open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema

type public LocationDetail (latitude,longitude,title,address) =
    let mutable latitude = latitude
    let mutable longitude = longitude
    let mutable title = title
    let mutable address = address
    
    member public this.Latitude
        with get() = latitude
        and set(value) = latitude <- value    
    
    member public this.Longitude
        with get() = longitude
        and set(value) = longitude <- value    
        
    member public this.Title
        with get() = title
        and set(value) = title <- value    

    member public this.Address
        with get() = address
        and set(value) = address <- value    

type public RSVP () =
    let mutable rsvpID = 0
    let mutable dinnerID = 0
    let mutable attendeeName = ""
    let mutable attendeeNameId = ""
    let mutable dinner = null

    member public self.RsvpID
        with get() = rsvpID
        and set(value) = rsvpID <- value

    member public self.DinnerID
        with get() = dinnerID
        and set(value) = dinnerID <- value

    member public self.AttendeeName
        with get() = attendeeName
        and set(value) = attendeeName <- value

    member public self.AttendeeNameId
        with get() = attendeeNameId
        and set(value) = attendeeNameId <- value

    member public self.Dinner
        with get() = dinner
        and set(value) = dinner <- value


and public Dinner () =
    let mutable dinnerID = 0
    let mutable title = ""
    let mutable eventDate = DateTime.MinValue
    let mutable description = ""
    let mutable hostedBy = ""
    let mutable contactPhone = ""
    let mutable address = ""
    let mutable country = ""
    let mutable latitude = 0.
    let mutable longitude = 0.
    let mutable hostedById = ""
    let mutable rsvps = List<RSVP>() :> ICollection<RSVP> 
    
    [<HiddenInput(DisplayValue=false)>]
    member public self.DinnerID
        with get() = dinnerID
        and set(value) = dinnerID <- value

    [<Required(ErrorMessage="Title Is Required")>]
    [<StringLength(50,ErrorMessage="Title may not be longer than 50 characters")>]
    member public self.Title
        with get() = title
        and set(value) = title <- value

    [<Required(ErrorMessage="EventDate Is Required")>]
    [<Display(Name="Event Date")>]
    member public self.EventDate
        with get() = eventDate
        and set(value) = eventDate <- value

    [<Required(ErrorMessage="Description Is Required")>]
    [<StringLength(256,ErrorMessage="Description may not be longer than 256 characters")>]
    [<DataType(DataType.MultilineText)>]
    member public self.Description
        with get() = description
        and set(value) = description <- value

    [<StringLength(256,ErrorMessage="Hosted By may not be longer than 256 characters")>]
    [<Display(Name="Hosted By")>]
    member public self.HostedBy
        with get() = hostedBy
        and set(value) = hostedBy <- value

    [<Required(ErrorMessage="Contact Phone Is Required")>]
    [<StringLength(20,ErrorMessage="Contact Phone may not be longer than 20 characters")>]
    [<Display(Name="Contact Phone")>]
    member public self.ContactPhone
        with get() = contactPhone
        and set(value) = contactPhone <- value

    [<Required(ErrorMessage="Address Is Required")>]
    [<StringLength(20,ErrorMessage="Address may not be longer than 50 characters")>]
    [<Display(Name="Address")>]
    member public self.Address
        with get() = address
        and set(value) = address <- value    

    [<UIHint("CountryDropDown")>]
    member public this.Country
        with get() = country
        and set(value) = country <- value    

    [<HiddenInput(DisplayValue=false)>]
    member public self.Latitude
        with get() = latitude
        and set(value) = latitude <- value    
    
    [<HiddenInput(DisplayValue=false)>]
    member public v.Longitude
        with get() = longitude
        and set(value) = longitude <- value    

    [<HiddenInput(DisplayValue=false)>]
    member public self.HostedById
        with get() = hostedById
        and set(value) = hostedById <- value    

    member public self.RSVPs
        with get() = rsvps
        and set(value) = rsvps <- value    

    member public self.IsHostedBy (userName:string) =
        System.String.Equals(hostedBy,userName,System.StringComparison.Ordinal)

    member public self.IsUserRegistered(userName:string) =
        rsvps |> Seq.exists(fun r -> r.AttendeeName = userName)
              

    [<UIHint("Location Detail")>]
    [<NotMapped()>]
    member public self.Location
        with get() = new LocationDetail(self.Latitude,self.Longitude,self.Title,self.Address)
        and set(value:LocationDetail) = 
            let latitude = value.Latitude
            let longitude = value.Longitude
            let title = value.Title
            let address = value.Address
            ()
                          
type nameMappingContext = CsvProvider<"C:/data/nameList.csv">

type AdProvider () =
    member this.GetCatagory personName: string =
        let nameList = nameMappingContext.Load("C:/data/nameList.csv")
        let foundName = nameList.Rows
                            |> Seq.filter(fun r -> r.Annie = personName)
                            |> Seq.map(fun r -> r.OldFemale)
                            |> Seq.toArray
        if foundName.Length > 0 then
            foundName.[0]
        else
            "middleAgedMale"

open System
open System.Collections.Generic
open System.IO
open System.Net.Http
open System.Net.Http.Formatting
open System.Net.Http.Headers
open System.Text
open System.Threading.Tasks


type ScoreData (featureVector:Dictionary<string,string>, globalParameters:Dictionary<string,string>) = 
    member this.FeatureVector = featureVector
    member this.GlobalParameters = globalParameters
 
type ScoreRequest (id:string, instance:ScoreData) = 
    member this.Id = id
    member this.Instance = instance


type AttendancePredictor () =
    let invokeService (hostName, dayOfWeek) = async {
        let apiKey = "NVOnw/2EXdaOhwZXb0n6jFa01vIHBMNb/42RaObcnQRTSVdOH/YEeSK7YGp3vn3s7BQ5dwUmJmrxj7+U5oKF+Q=="
        let uri = "https://ussouthcentral.services.azureml.net/workspaces/8d32705e228247c7b2f14301c2158a99/services/8784eaff32a54531a6f49bda81e9f408/score"
        use client = new HttpClient()
        client.DefaultRequestHeaders.Authorization <- new AuthenticationHeaderValue("Bearer",apiKey)
        client.BaseAddress <- new Uri(uri)
 
        let input = new Dictionary<string,string>()
        input.Add("Organizer",hostName)
        input.Add("DayOfWeek",dayOfWeek)
        input.Add("NumberOfAttendees","0")
 
        let scoreData = new ScoreData(input,new Dictionary<string,string>())
        let scoreRequest = new ScoreRequest("score00001",scoreData)
        let! response = client.PostAsJsonAsync("",scoreRequest) |> Async.AwaitTask
        let! result = response.Content.ReadAsStringAsync() |> Async.AwaitTask
 
        if response.IsSuccessStatusCode then
            printfn "%s" result
        else
            printfn "FAILED: %s" result
        response |> ignore
        return result
    }

    member this.GetProjectedAttendance(hostName, dayOfWeek) =
        let returnString = invokeService(hostName, dayOfWeek) |> Async.RunSynchronously
        let returnString' = returnString.Replace('[',' ')
        let returnString'' = returnString'.Replace(']',' ')
        let returnString''' = returnString''.Trim()
        let returnString'''' = returnString'''.Replace('"',' ')
        let stringArray = returnString''''.Split(',')
        let projectedCount = stringArray.[3].Trim()
        let projectedCount' = Double.Parse(projectedCount)
        (int)projectedCount'

