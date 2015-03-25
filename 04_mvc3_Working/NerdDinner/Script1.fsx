//01_Setup
#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
#r "../packages/FSharp.Charting.0.90.7/lib/net40/FSharp.Charting.dll"
#r "C:/Program Files (x86)/Reference Assemblies/Microsoft/Framework/.NETFramework/v4.5.1/System.Windows.Forms.DataVisualization.dll"

open FSharp.Data
open FSharp.Charting

//http://www.ssa.gov/OACT/babynames/limits.html
//http://www.fonz.net/blog/archives/2008/04/06/csv-of-states-and-state-abbreviations/

//type censusDataContext = CsvProvider<"https://portalvhdspgzl51prtcpfj.blob.core.windows.net/censuschicken/AK.TXT">
//type censusDataContext = CsvProvider<"C:/data/namesbystate/AK.TXT",HasHeaders = false>

type censusDataContext = CsvProvider<"C:/data/namesbystate/AK.TXT">
type stateCodeContext = CsvProvider<"https://portalvhdspgzl51prtcpfj.blob.core.windows.net/censuschicken/states.csv">

//02_State Codes 
let stateCodes =  stateCodeContext.Load("https://portalvhdspgzl51prtcpfj.blob.core.windows.net/censuschicken/states.csv");

//let uri = System.String.Format("https://portalvhdspgzl51prtcpfj.blob.core.windows.net/censuschicken/{0}.TXT",stateCode)
let fetchStateData (stateCode:string)=
        let uri = System.String.Format("C:/data/namesbystate/{0}.TXT",stateCode)
        censusDataContext.Load(uri)


//03_All Census Data
let usaData = stateCodes.Rows 
                |> Seq.collect(fun r -> fetchStateData(r.Abbreviation).Rows)
                |> Seq.toArray

//04_sum up a name across all states
let nameSum = usaData 
                |> Seq.groupBy(fun r -> r.Mary)
                |> Seq.map(fun (n,a) -> n,a |> Seq.sumBy(fun (r) -> r.``14``)) 
                |> Seq.toArray

//05_sum up all records in the USA
let totalNames = nameSum |> Seq.sumBy(fun (n,c) -> c)

//06_See the most popular names descending
let nameAverage = nameSum 
                    |> Seq.map(fun (n,c) -> n,c,float c/ float totalNames)
                    |> Seq.sortBy(fun (n,c,a) -> -a - 1.)
                    |> Seq.toArray        

//07make a function to see names, split by gender
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

//08make a function to see names, split by year of birth
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


//09Chart the year of birth
let chartData = ageSearch "Jose"
                    |> Seq.map(fun (y,c,p) -> y, c)
                    |> Seq.sortBy(fun (y,c) -> y)
    
Chart.Line(chartData).ShowChart()

//10basic stats on Name - average, min ,max

//11Variance
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

//12Average and Attachment Point
let average name = ageSearch name
                    |> Seq.map(fun (y,c,p) -> float c)
                    |> Seq.average
average "James"

let attachmentPoint name = average "James" + standardDeviation' "James"

attachmentPoint "James"

//13PopularYears
let popularYears name = 
    let allYears = ageSearch name
    let attachmentPoint' = attachmentPoint name
    let filteredYears = allYears 
                        |> Seq.filter(fun (y,c,p) -> float c > attachmentPoint')
                        |> Seq.sortBy(fun (y,c,p) -> y)
    filteredYears


let jamesPopular = popularYears "James"
jamesPopular

//14First and Last Popular Year
let lastPopularYear name = popularYears name |> Seq.last
let firstPopularYear name = popularYears name |> Seq.head

lastPopularYear "James"
firstPopularYear "James"

//15StateSearch
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

//16State Chart
let chartData' = stateSearch "Jose"
                    |> Seq.map(fun (s,c,p) -> s,c)
    
Chart.Column(chartData').ShowChart()


//17Quartiles

//18Just doing age and gender b/c state is too inclusive
let nameAssignment (malePercent, lastYearPopular) =
    match malePercent > 0.75, malePercent < 0.75, lastYearPopular < 1945, lastYearPopular > 1980 with
        | true, false, true, false -> "oldMale"
        | true, false, false, false -> "middleAgedMale"
        | true, false, false, true -> "youngMale"
        | false, true, true, false -> "oldFemale"
        | false, true, false, false -> "middleAgedFemale"
        | false, true, false, true -> "youngFemale"
        | _,_,_,_ -> "unknown"


//19Gender Search -> Only Male
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

//20Last Popular Years -> Including Choice Type
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

//21Create Name List
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
//22Write To File
open System.IO
let outFile = new StreamWriter(@"c:\data\nameList3.csv")
nameList'' |> Seq.iter(fun (n,c) -> outFile.WriteLine(sprintf "%s,%s" n c))
outFile.Flush
outFile.Close()
