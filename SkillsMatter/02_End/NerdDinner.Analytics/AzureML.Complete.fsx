//Drag in a reader and point to:
//server: nc54a9m5kk.database.windows.net
//Database: NerdDinner
//User Account: chickenskills@nc54a9m5kk
//Password: sk1lzm@tter
//Use this Sql
//Select
//HostedById as HostId,
//DATEPART(weekday,EventDate) as DayOfWeek,
//Count(RSVPID) as Attendance
//from [dbo].[Dinners] as d
//Inner join [dbo].[RSVPs] as r
//on d.DinnerID = r.DinnerID
//Group By HostedById, EventDate

//drag in a metadata editor to make Attendance an int
//drag in a metadata editors to make HostId,DayOfWeek categorical
//drag in a split
//drag in a model
//drag in a train - predict attendance
//drag in a score
//drag in an evaluate
//run it
//Set up as a predictive web service
//run it
//copy uri and apiKey to use below

#r "System.Net.Http.dll"
#r "../packages/Newtonsoft.Json.7.0.1/lib/net40/Newtonsoft.Json.dll"
 
open System
open System.Net
open Newtonsoft.Json
open System.Net.Http
open System.Net.Http.Headers
open System.Collections.Generic

type StringTable(columnNames:array<string>, values:array<array<string>>) =
    member this.ColumnNames = columnNames
    member this.Values = values

type ScoreRequest (inputs:Dictionary<string,StringTable>, globalParameters:Dictionary<string,string>) = 
    member this.Inputs = inputs
    member this.GlobalParameters = globalParameters
    
type Value(columnNames:array<string>, columnTypes:array<string>, values:array<array<string>>) =
    member this.ColumnNames = columnNames
    member this.ColumnTypes = columnTypes
    member this.Values = values

type Output1(outputType:string, value:Value) =
    member this.OutputType = outputType
    member this.Value = value

type Results(output1: Output1) =
    member this.Output = output1

type ScoreResponse(results:Results) =
    member this.Results = results

let apiKey = "IK+rd6ekNSJpJ+5yfdKG3BUJFdJHOt8+OE/pxA40mEewXq4gFceefoJI1E0f/XpqbWCCALD2NDr6JIX+DZC9VA=="
let uri = "https://ussouthcentral.services.azureml.net/workspaces/8d32705e228247c7b2f14301c2158a99/services/ab3279d700b246afb16c5ba6ce4cbf04/execute?api-version=2.0&details=true"
 
let columnNames = [|"HostId";"DayOfWeek";"Attendance"|]
let values = [|[|"scottgu";"5";"0"|]|] 
let input = new StringTable(columnNames,values)
let inputs = new Dictionary<string,StringTable>()
inputs.Add("input1", input)

let scoreRequest = new ScoreRequest(inputs,new Dictionary<string,string>())
let json = JsonConvert.SerializeObject(scoreRequest); 
let client = new WebClient()
client.Headers.Add("Authorization","Bearer " + apiKey)
client.Headers.Add("Content-Type", "application/json")
let resultJson = client.UploadString(uri,json)
let result = JsonConvert.DeserializeObject<ScoreResponse>(resultJson)
let reultValues = Seq.last result.Results.Output.Value.Values 
Seq.last  reultValues


