#r @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Net.Http.dll"
#r @"..\packages\Microsoft.AspNet.WebApi.Client.5.2.2\lib\net45\System.Net.Http.Formatting.dll"
 
open System
open System.Net.Http
open System.Net.Http.Headers
open System.Net.Http.Formatting
open System.Collections.Generic
 
type ScoreData (featureVector:Dictionary<string,string>, globalParameters:Dictionary<string,string>) = 
    member this.FeatureVector = featureVector
    member this.GlobalParameters = globalParameters
 
type ScoreRequest (id:string, instance:ScoreData) = 
    member this.Id = id
    member this.Instance = instance
 
let invokeService () = async {
    let apiKey = "NVOnw/2EXdaOhwZXb0n6jFa01vIHBMNb/42RaObcnQRTSVdOH/YEeSK7YGp3vn3s7BQ5dwUmJmrxj7+U5oKF+Q=="
    let uri = "https://ussouthcentral.services.azureml.net/workspaces/8d32705e228247c7b2f14301c2158a99/services/8784eaff32a54531a6f49bda81e9f408/score"
    use client = new HttpClient()
    client.DefaultRequestHeaders.Authorization <- new AuthenticationHeaderValue("Bearer",apiKey)
    client.BaseAddress <- new Uri(uri)
 
    let input = new Dictionary<string,string>()
    input.Add("Organizer","scottgu")
    input.Add("DayOfWeek","Saturday")
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
 
let returnString = invokeService() 
                    |> Async.RunSynchronously

let returnString' = returnString.Replace('[',' ')
let returnString'' = returnString'.Replace(']',' ')
let returnString''' = returnString''.Trim()
let returnString'''' = returnString'''.Replace('"',' ')
let stringArray = returnString''''.Split(',')
let projectedCount = stringArray.[3].Trim()
let projectedCount' = Double.Parse(projectedCount)
(int)projectedCount'

