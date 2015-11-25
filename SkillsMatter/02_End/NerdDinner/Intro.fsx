//Install FSLab via Nuget
//PM> Install-Package FsLab


//Get data via Type Provider
//Transform using Array.Map
//Create a Tuple
#r "../packages/FSharp.Data.2.2.5/lib/net40/FSharp.Data.dll"
open System
open FSharp.Data

let tickers = [|"MSFT";"IBM";"GOOG"|]

let getData(ticker:string) =
    let file =CsvFile.Load("http://ichart.finance.yahoo.com/table.csv?s="+ticker)
    file.Rows |> Seq.map(fun r -> ticker,r)
              |> Seq.toArray 

//Array.Collect
let allData = Array.collect(fun t -> getData(t)) tickers

//Array.GroupBy
allData |> Array.groupBy(fun (t, r) -> t )

//Array.SumBy
allData |> Array.sumBy(fun (t, r) -> Int64.Parse(r.Item(5)))

//Function Chaining
allData |> Array.groupBy(fun (t, r) -> t )
        |> Seq.map(fun (t,a) -> t, a |> Seq.sumBy(fun (t,r) -> Int64.Parse(r.Item(5))))

//Sort By and Head
allData |> Array.sortByDescending(fun (t,r) -> r.Item(5))
        |> Seq.head

//Filter
allData |> Array.filter(fun (t,r) -> t = "MSFT")

//Distinct
allData |> Array.distinctBy(fun (t,r) -> t)

//Tuple item selection - fst and snd
allData |> Array.map(fun t -> fst t)
allData |> Array.map(fun t -> snd t)

//2 similar functions combined using high-order function
let getOpenPrices = allData |> Array.map(fun (t,r) -> r.Item(1))
let getClosePrices = allData |> Array.map(fun (t,r) -> r.Item(3))

let getPrices mapper = allData |> Array.map(mapper)
getPrices(fun (t,r) -> r.Item(1))
getPrices(fun (t,r) -> r.Item(3))

//Prices Over Time Using A Chart
#load "../packages/FSharp.Charting.0.90.13/FSharp.Charting.fsx"
open FSharp.Charting

let chartData = allData |> Array.map(fun (t,r) -> r.Item(1), r.Item(2))
Chart.Line(chartData)

//Descriptive stats: min, max, mean, standard deviation
allData |> Array.filter(fun (t,r) -> t = "MSFT")
        |> Array.map(fun (t,r) -> float(r.Item(5)))
        |> Array.min

allData |> Array.filter(fun (t,r) -> t = "MSFT")
        |> Array.map(fun (t,r) -> float(r.Item(5)))
        |> Array.max

allData |> Array.filter(fun (t,r) -> t = "MSFT")
        |> Array.map(fun (t,r) -> float(r.Item(5)))
        |> Array.average

//http://www.codeproject.com/Articles/42492/Using-LINQ-to-Calculate-Basic-Statistics
//http://www.mathsisfun.com/data/standard-deviation.html

let variance (values:float seq) =
    let mean = Seq.average values
    let deltas = Seq.map(fun x -> pown(x-mean) 2) values
    Seq.average deltas

let standardDeviation(values:float seq) =
    sqrt(variance(values))

allData |> Array.filter(fun (t,r) -> t = "MSFT")
        |> Array.map(fun (t,r) -> float(r.Item(5)))
        |> standardDeviation

let attachmentPoint (values:float seq) =
    let average = Seq.average values
    let standardDeviation = standardDeviation values
    average + standardDeviation

let attachmentPoint' = allData |> Array.filter(fun (t,r) -> t = "MSFT")
                               |> Array.map(fun (t,r) -> float(r.Item(5)))
                               |> attachmentPoint

let maxDays = allData |> Array.filter(fun (t,r) -> t = "MSFT" && Double.Parse(r.Item(5)) > attachmentPoint')
maxDays |> Array.last, maxDays |> Array.head

//Quartiles
//http://www.mathsisfun.com/data/quartiles.html
let msftDays = allData |> Array.filter(fun (t,r) -> t = "MSFT")
                       |> Array.sortByDescending(fun (t,r) -> r.Item(5))
let totalDays = Array.length msftDays
let topQuarterileIndex = totalDays/4
let topMsftDays = Array.take(topQuarterileIndex) msftDays 

let topQuartileTotal = topMsftDays 
                        |> Array.map(fun (t,r) ->  Double.Parse(r.Item(5)))
                        |> Array.sum

let allTotal = msftDays
                |> Array.map(fun (t,r) ->  Double.Parse(r.Item(5)))
                |> Array.sum

topQuartileTotal/allTotal

//Union Types, Choice Types, and Pattern Matching
type PriceMovement =
| Bullish
| Bearish

let getPriceMovement priceChange =
    match priceChange with
    | 0.0 -> None
    | bull when priceChange > 0.0 -> Some Bullish
    | bear when priceChange < 0.0 -> Some Bearish 

allData |> Array.map(fun (t,r) -> t,r, getPriceMovement(float(r.Item(6)) - float(r.Item(1))))


//Write to disk
let testData = allData |> Array.filter(fun (t,r) -> t = "MSFT")
                         |> Array.map(fun (t,r) -> t, r.Item(5).ToString())

open System.IO
let baseDirectory = System.IO.DirectoryInfo(__SOURCE_DIRECTORY__)
let dataDirectory = baseDirectory.Parent.Parent.FullName + @"\Data"
let outFile = new StreamWriter(dataDirectory + @"\testFile.csv")
testData |> Seq.iter(fun (n,c) -> outFile.WriteLine(sprintf "%s,%s" n c))
outFile.Flush
outFile.Close()

                