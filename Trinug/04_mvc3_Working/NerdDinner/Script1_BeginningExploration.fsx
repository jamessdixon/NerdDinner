#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
#r "../packages/FSharp.Charting.0.90.7/lib/net40/FSharp.Charting.dll"
#r "C:/Program Files (x86)/Reference Assemblies/Microsoft/Framework/.NETFramework/v4.5.1/System.Windows.Forms.DataVisualization.dll"

open FSharp.Data
open FSharp.Charting

//http://www.ssa.gov/OACT/babynames/limits.html
//http://www.fonz.net/blog/archives/2008/04/06/csv-of-states-and-state-abbreviations/

//type censusDataContext = CsvProvider<"https://portalvhdspgzl51prtcpfj.blob.core.windows.net/censuschicken/AK.TXT">
//type censusDataContext = CsvProvider<"C:/data/namesbystate/AK.TXT",HasHeaders = false>

//type stateCodeContext = CsvProvider<"https://portalvhdspgzl51prtcpfj.blob.core.windows.net/censuschicken/states.csv"> 
//let stateCodes =  stateCodeContext.Load("https://portalvhdspgzl51prtcpfj.blob.core.windows.net/censuschicken/states.csv");
type stateCodeContext = CsvProvider<"C:/data/states.csv"> 
let stateCodes =  stateCodeContext.Load("C:/data/states.csv");

//let uri = System.String.Format("https://portalvhdspgzl51prtcpfj.blob.core.windows.net/censuschicken/{0}.TXT",stateCode)
type censusDataContext = CsvProvider<"C:/data/namesbystate/AK.TXT">
let fetchStateData (stateCode:string)=
        let uri = System.String.Format("C:/data/namesbystate/{0}.TXT",stateCode)
        censusDataContext.Load(uri)
 
//put all of the data into 1 array
let usaData = stateCodes.Rows 
                |> Seq.collect(fun r -> fetchStateData(r.Abbreviation).Rows)
                |> Seq.toArray

//sum up a name across all states  
let nameSum = usaData 
                |> Seq.groupBy(fun r -> r.Mary)
                |> Seq.map(fun (n,a) -> n,a |> Seq.sumBy(fun (r) -> r.``14``)) 
                |> Seq.toArray
 
//sum up all records in the USA
let totalNames = nameSum |> Seq.sumBy(fun (n,c) -> c)
 
//See the most popular names descending
let nameAverage = nameSum 
                    |> Seq.map(fun (n,c) -> n,c,float c/ float totalNames)
                    |> Seq.sortBy(fun (n,c,a) -> -a - 1.)
                    |> Seq.toArray        


//make a function to see names, split by gender
let genderSearch name = 
    let nameFilter = usaData
                        |> Seq.filter(fun r -> r.Mary = name)
                        |> Seq.groupBy(fun r -> r.F)
                        |> Seq.map(fun (n,a) -> n,a |> Seq.sumBy(fun (r) -> r.``14``)) 
 
    let nameSum = nameFilter |> Seq.sumBy(fun (n,c) -> c)
    nameFilter 
        |> Seq.map(fun (n,c) -> n, c, float c/float nameSum) 
        |> Seq.toArray

genderSearch "James"

//make a function to see names, split by year of birth
let ageSearch name =
    let nameFilter = usaData
                        |> Seq.filter(fun r -> r.Mary = name)
                        |> Seq.groupBy(fun r -> r.``1910``)
                        |> Seq.map(fun (n,a) -> n,a |> Seq.sumBy(fun (r) -> r.``14``)) 
                        |> Seq.toArray
    let nameSum = nameFilter |> Seq.sumBy(fun (n,c) -> c)
    nameFilter 
        |> Seq.map(fun (n,c) -> n, c, float c/float nameSum) 
        |> Seq.toArray

ageSearch "James"

//Chart the year of birth
let chartData = ageSearch "Tom"
                    |> Seq.map(fun (y,c,p) -> y, c)
                    |> Seq.sortBy(fun (y,c) -> y)
    
Chart.Line(chartData).ShowChart()

//basic stats on Name - average, min ,max
ageSearch "James"
    |> Seq.map(fun (y,c,p) -> float c)
    |> Seq.average

ageSearch "James"
    |> Seq.map(fun (y,c,p) -> float c)
    |> Seq.min

ageSearch "James"
    |> Seq.map(fun (y,c,p) -> float c)
    |> Seq.max

//Variance
//http://www.mathsisfun.com/data/standard-deviation.html
let variance (source:float seq) =
    let mean = Seq.average source
    let deltas = Seq.map(fun x -> pown(x-mean) 2) source
    Seq.average deltas

let standardDeviation(values:float seq) =
    sqrt(variance(values))

let standardDeviation' name = ageSearch name
                                |> Seq.map(fun (y,c,p) -> float c)
                                |> standardDeviation
standardDeviation' "James"

//Average and Attachment Point
let average name = ageSearch name
                    |> Seq.map(fun (y,c,p) -> float c)
                    |> Seq.average
average "James"

let attachmentPoint name = average "James" + standardDeviation' "James"

attachmentPoint "James"

//PopularYears
let popularYears name = 
    let allYears = ageSearch name
    let attachmentPoint' = attachmentPoint name
    let filteredYears = allYears 
                        |> Seq.filter(fun (y,c,p) -> float c > attachmentPoint')
                        |> Seq.sortBy(fun (y,c,p) -> y)
    filteredYears


let jamesPopular = popularYears "James"
jamesPopular

//First and Last Popular Year
let lastPopularYear name = popularYears name |> Seq.last
let firstPopularYear name = popularYears name |> Seq.head

lastPopularYear "James"
firstPopularYear "James"


//StateSearch
let stateSearch name =
    let nameFilter = usaData
                        |> Seq.filter(fun r -> r.Mary = name)
                        |> Seq.groupBy(fun r -> r.AK)
                        |> Seq.map(fun (n,a) -> n,a |> Seq.sumBy(fun (r) -> r.``14``)) 
                        |> Seq.toArray
    let nameSum = nameFilter |> Seq.sumBy(fun (n,c) -> c)
    nameFilter 
        |> Seq.map(fun (n,c) -> n, c, float c/float nameSum) 
        |> Seq.toArray

//State Chart
let chartData' = stateSearch "Jamie"
                    |> Seq.map(fun (s,c,p) -> s,c)
    
Chart.Column(chartData').ShowChart()

//Quartiles
//http://www.mathsisfun.com/data/quartiles.html
let topQuartileStates = stateSearch "James"
                            |> Seq.sortBy(fun (s,c,p) -> -c-1)
                            |> Seq.take (50/4)

let topQuartileTotal = topQuartileStates 
                            |> Seq.sumBy(fun (s,c,p) -> c)

let total = stateSearch "James"
                |> Seq.sumBy(fun (s,c,p) -> c)

float topQuartileTotal/float total

//Make a local file
//Just doing age and gender b/c state is too inclusive
let nameAssignment (malePercent, lastYearPopular) =
    match malePercent > 0.75, malePercent < 0.75, lastYearPopular < 1945, lastYearPopular > 1980 with
        | true, false, true, false -> "oldMale"
        | true, false, false, false -> "middleAgedMale"
        | true, false, false, true -> "youngMale"
        | false, true, true, false -> "oldFemale"
        | false, true, false, false -> "middleAgedFemale"
        | false, true, false, true -> "youngFemale"
        | _,_,_,_ -> "unknown"

//Gender Search -> Only Male
let genderSearch' name = 
    let nameFilter = usaData
                        |> Seq.filter(fun r -> r.Mary = name)
                        |> Seq.groupBy(fun r -> r.F)
                        |> Seq.map(fun (n,a) -> n,a |> Seq.sumBy(fun (r) -> r.``14``)) 
 
    let nameSum = nameFilter |> Seq.sumBy(fun (n,c) -> c)
    let nameFilter' = nameFilter
                        |> Seq.map(fun (n,c) -> n, c, float c/float nameSum) 
                        |> Seq.filter(fun (g,c,p) -> g = "M")
    if nameFilter' = Seq.empty then
            0.       
        else
        nameFilter'
            |> Seq.map(fun (g,c,p) -> p)
            |> Seq.head

genderSearch' "James"

//Last Popular Years -> Including Choice Type
let lastPopularYear' name = 
    let popularYears' = popularYears name 
    if popularYears' = Seq.empty
        then None
    else
        let last = popularYears' 
                    |> Seq.last
        let year, _, _ = last
        Some year

lastPopularYear' "James"

//Create Name List
let nameList = nameAverage
                |> Seq.map(fun (n,c,p) -> n)
                |> Seq.distinct

let nameList' = nameList
                |> Seq.map(fun n -> n, genderSearch' n)
                |> Seq.map(fun (n,mp) -> n,mp, lastPopularYear n)
                |> Seq.map(fun (n,mp,(y,c,p)) -> n, mp, y)
                |> Seq.map(fun (n,mp,y) -> n,nameAssignment(mp,y))
                |> Seq.toList

let nameList'' = nameList' |> Seq.map(fun (n,c) -> n,c.ToString())

//Write To File
open System.IO
let outFile = new StreamWriter(@"c:\data\nameList3.csv")
nameList'' |> Seq.iter(fun (n,c) -> outFile.WriteLine(sprintf "%s,%s" n c))
outFile.Flush
outFile.Close()



