
namespace NerdDinner.Providers

open System
open FSharp.Data

type AdProvider (fileUri:string) =
    member this.GetCatagory personName: string =
        let nameList = CsvFile.Load(fileUri)
        let foundName = nameList.Rows
                            |> Seq.filter(fun r -> r.Item(0) = personName)
                            |> Seq.map(fun r -> r.Item(1))
                            |> Seq.toArray
        if foundName.Length > 0 then
            foundName.[0]
        else
            "middleAgedMale"

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


type AttendancePredictor (apiKey:string, uri: string) =

    member this.GetProjectedAttendance(hostId, dayOfWeek) =
        let columnNames = [|"HostId";"DayOfWeek";"Attendance"|]
        let values = [|[|hostId;dayOfWeek;"0"|]|] 
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
