

//http://archive.ics.uci.edu/ml/datasets/Iris
//Drag in a reader and point to: http://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data
//drag in a metadata editor to make new column names: sepalLength,sepalWidth,petalLength,petalWidth,class
//drag in a metadata editor to make sepalLength,sepalWidth,petalLength,petalWidth a float
//drag in a metadata editors to make class categorical
//drag in a split
//drag in a model
//drag in a train - predict petalWidth
//drag in a score
//drag in an evaluate
//Set up as a predictive web service
//run it
//copy uri and apiKey

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

let apiKey = "RzXsY9xTe6+/O//fHs67G+EDCrQFuK2odn7TaRZYobTTcxKgctqYvBrbAJyZuIZ0awO/4Rdp846ptHGQWV6+fg=="
let uri = "https://ussouthcentral.services.azureml.net/workspaces/8d32705e228247c7b2f14301c2158a99/services/6e64459224704560bf6aa7d6dec6a847/execute?api-version=2.0&details=true"
 
let columnNames = [|"Col1";"Col2";"Col3";"Col4";"Col5"|]
//Col4 is petal width, which we are predicting
let values = [|[|"5.1";"3.5";"1.4";"0.0";"Iris-setosa"|]|] 
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
let resultValues = Seq.last result.Results.Output.Value.Values 
Seq.last  resultValues


