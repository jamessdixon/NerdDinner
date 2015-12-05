
#r "../packages/FSharp.Data.2.2.5/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data

//1) Bring in the data from disk (Data folder)
//Create 1 large dataframe with all of the state data
let baseDirectory = System.IO.DirectoryInfo(__SOURCE_DIRECTORY__)
let dataDirectory = baseDirectory.Parent.Parent.FullName + @"\Data"

let stateCodes = CsvFile.Load(dataDirectory + @"\State Code\states.csv")

let fetchStateData (stateCode:string) =
    let uri = dataDirectory + @"\SSA\" + stateCode + ".TXT"
    CsvFile.Load(uri).Rows

let usaData = stateCodes.Rows
              |> Seq.collect(fun r -> fetchStateData(r.Item(1)))
              |> Seq.toArray

//2) Sum up names regardless of state or year
let nameSum = usaData |> Seq.groupBy(fun r -> r.Item(3))
                      |> Seq.map(fun (n,a) -> n, a |> Seq.sumBy(fun r -> Int32.Parse(r.Item(4))))
                      |> Seq.toArray

//2) Sum up all records in the USA
let totalNames = Seq.sumBy(fun (n,c) -> c) nameSum

//3) Most popular names?
nameSum |> Seq.map(fun (n,c) -> n, c, float c/float totalNames)
        |> Seq.sortByDescending(fun (n,c,p) -> p)

//4) Make a function to see how a name splits by gender
let genderSearch name =
    let filter = 
        usaData |> Seq.filter(fun r -> r.Item(3) = name)
                |> Seq.groupBy(fun r -> r.Item(1))
                |> Seq.map(fun (g,a) -> g,a |> Seq.sumBy(fun (r) -> Int32.Parse(r.Item(4))))
                |> Seq.sortBy(fun (g,c) -> g)
    let total = filter |> Seq.sumBy(fun (g,c) -> c) 
    filter |> Seq.map(fun (g,c) -> g,c, float c/float total)

genderSearch "James" 

//4) Make a function to see how a name splits by year of birth
let yearBirth name =
    let filter = 
        usaData |> Seq.filter(fun r -> r.Item(3) = name)
                |> Seq.groupBy(fun r -> r.Item(2))
                |> Seq.map(fun (y,a) -> y,a |> Seq.sumBy(fun (r) -> Int32.Parse(r.Item(4))))
                |> Seq.sortBy(fun (y,c) -> y)
    let total = filter |> Seq.sumBy(fun (y,c) -> c) 
    filter |> Seq.map(fun (y,c) -> y,c, float c/float total)

yearBirth "James" 

//5) Combine the functions using a high order function
let nameQuery name grouper =
    let filter = 
        usaData |> Seq.filter(fun r -> r.Item(3) = name)
                |> Seq.groupBy(grouper)
                |> Seq.map(fun (i,a) -> i,a |> Seq.sumBy(fun (r) -> Int32.Parse(r.Item(4))))
                |> Seq.sortBy(fun (i,c) -> i)
    let total = filter |> Seq.sumBy(fun (i,c) -> c) 
    filter |> Seq.map(fun (i,c) -> i,c, float c/float total)

nameQuery "James" (fun r -> r.Item(1))
nameQuery "James" (fun r -> r.Item(2))

//6) See how the name looks over the years using a chart
#load "../packages/FSharp.Charting.0.90.13/FSharp.Charting.fsx"
open FSharp.Charting

let chartData = nameQuery "James" (fun r -> r.Item(2))
                |> Seq.map(fun (y,c,p) -> y,c)
Chart.Line(chartData)

//7) Calculate basic statiistics for a name - Min.Max,Mean
usaData |> Array.filter(fun r -> r.Item(3) = "James")
        |> Array.map(fun r -> r.Item(4))
        |> Array.min

usaData |> Array.filter(fun r -> r.Item(3) = "James")
        |> Array.map(fun r -> float(r.Item(4)))
        |> Array.max

usaData |> Array.filter(fun r -> r.Item(3) = "James")
        |> Array.map(fun r -> float(r.Item(4)))
        |> Array.average


//8) Using the attachment point, determine the popular years for a name
let variance (values:float seq) =
    let mean = Seq.average values
    let deltas = Seq.map(fun x -> pown(x-mean) 2) values
    Seq.average deltas

let standardDeviation(values:float seq) =
    sqrt(variance(values))

let attachmentPoint (values:float seq) =
    let average = Seq.average values
    let standardDeviation = standardDeviation values
    average + standardDeviation

let popularYears name =
    let filter = 
        usaData |> Seq.filter(fun r -> r.Item(3) = name)
                |> Seq.groupBy(fun r -> r.Item(2))
                |> Seq.map(fun (y,a) -> y,a |> Seq.sumBy(fun (r) -> Int32.Parse(r.Item(4))))
                |> Seq.sortBy(fun (y,c) -> y)

    let attachmentPoint' = attachmentPoint(filter |> Seq.map(fun (y,c) -> float c))
    let filter' = filter |> Seq.filter(fun (y,c) -> float c > attachmentPoint')
    match Seq.length filter' with
    | 0 -> None
    | _ -> Some (Seq.head filter', Seq.last filter')

popularYears "James"

//9) Instead of year of birth, can the state of birth tell us anything?
let chartData' = nameQuery "James" (fun r -> r.Item(0))
                |> Seq.map(fun (s,c,p) -> s,c)
Chart.Bar(chartData')

//10) Quartiles
let topQuartileStates = nameQuery "James" (fun r -> r.Item(0))
                            |> Seq.sortByDescending(fun (s,c,p) -> p)
                            |> Seq.take (50/4)

let topQuartileTotal = topQuartileStates 
                            |> Seq.sumBy(fun (s,c,p) -> c)

let total = nameQuery "James" (fun r -> r.Item(0))
                |> Seq.sumBy(fun (s,c,p) -> c)

float topQuartileTotal/float total

//11) So we have somthing with age, but not really state

//Let's create a lookup file to be used in our application
//Assume any name whose gender is > 75% is should be catagorized as that gender
//For example, 'James' is 99% male, so it should be catagorized as 'Male'
//Also, assume anyone whose last year as a popular name was before 1945 is 'old'
//And anyone whose last popular year is after 1980 is 'young'
//Anyone in between is 'middle aged'
//So the last populate year for 'James' was 1964, si he would be catagorized
//as middke aged

//Make an age group and gender DU
//Make a function that calcualtes the ageband for a given name.
//If one is not avaiable, return a none
//If one is avaiable, reuturn theie age class
//Then do the same for gender
//Once you have age and gender class, return it as a tuple

type AgeGroup =
| Old
| MiddleAged
| Young

type Gender =
| Male
| Female

let getNameCategory name =
    let gender = nameQuery name (fun r -> r.Item(1))
                 |> Seq.map (fun (g,c,p) -> g,p)
    let head = Seq.head gender
    let gender' =  match snd head < 0.25, snd head > 0.75 with
                           | true, false -> match fst head with
                                                | "F" -> Some Male
                                                | _ -> Some Female
                           | false, true -> match fst head with
                                                | "F" -> Some Female
                                                | _ -> Some Male
                           | _, _ -> None

    let popularYears = (popularYears name)
    let ageGroup = match popularYears.IsSome with
                    | true -> 
                        let lastPopularYear = snd (popularYears.Value) 
                        match (fst lastPopularYear) < "1945", (fst lastPopularYear) > "1980" with
                        | true, _ -> Some Old
                        | _, true -> Some Young
                        | _,_ -> Some MiddleAged
                    | false -> None

    ageGroup,gender'

getNameCategory "James"

//12) Once you have the age/gender result for a name
//return it as a tuple with the name and a string description
//For example
//Some Old, Some Male = {Name} , Old Male'
let getNamePrintout name =
    match getNameCategory name with
    | None, _ -> name, "Unknown"
    | _, None ->  name, "Unknown"
    | Some Old, Some Male -> name, "Old Male"
    | Some Old, Some Female -> name, "Old Female"
    | Some MiddleAged, Some Male -> name, "MiddleAged Male"
    | Some MiddleAged, Some Female -> name, "MiddleAged Female"
    | Some Young, Some Male -> name, "Young Male"
    | Some Young, Some Female -> name, "Young Female"


//13) Write to disk
open System.IO
let outFile = new StreamWriter(dataDirectory + @"\nameLookup.csv")
nameSum |> Seq.map(fun (n,c) -> getNamePrintout n)
        |> Seq.iter(fun (n,s) -> outFile.WriteLine(sprintf "%s,%s" n s))
outFile.Flush
outFile.Close()