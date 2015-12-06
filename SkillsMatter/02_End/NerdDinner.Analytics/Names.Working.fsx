
#r "../packages/FSharp.Data.2.2.5/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data

//For reference
//http://www.ssa.gov/OACT/babynames/limits.html
//http://www.fonz.net/blog/archives/2008/04/06/csv-of-states-and-state-abbreviations/

//1) Bring in the data for Social Secourity Info and State Listings
//Create 1 large dataframe with all of the state data from Disk
//Data\\SSA\
//Data\State Code\states.csv
//
//val usaData : CsvRow [] =
//  [|[|"AL"; "F"; "1910"; "Annie"; "482"|];
//    [|"AL"; "F"; "1910"; "Willie"; "257"|];

//2) Sum up names regardless of state or year
//
//val nameSum : (string * int) [] =
//  [|("Annie", 275539); ("Willie", 532369);

//2) Sum up all records in the USA
//
//val totalNames : int = 298860400

//3) Most popular names?
//
//val it : seq<string * int * float> =
//  seq
//    [("James", 4957166, 0.01658689475); ("John", 4845414, 0.01621296766);
//     ("Robert", 4725713, 0.01581244287); 

//4) Make a function to see how a name splits by gender
//Filter
//GroupBy 
//Map  - SumBy
//SortBy
//
//val genderSearch : name:string -> seq<string * int * float>
//val it : seq<string * int * float> =
//  seq [("F", 18201, 0.003671654328); ("M", 4938965, 0.9963283457)]

//4) Make a function to see how a name splits by year of birth
//
//val yearBirth : name:string -> seq<string * int * float>
//val it : seq<string * int * float> =
//  seq
//    [("1910", 9203, 0.001856504301); 

//5) Combine the functions using a high order function
//
//val nameQuery :
//  name:string -> grouper:(CsvRow -> 'a) -> seq<'a * int * float>
//    when 'a : comparison
//val it : seq<string * int * float> =
//  seq [("F", 18201, 0.003671654328); ("M", 4938965, 0.9963283457)]
//> 
//val it : seq<string * int * float> =
//  seq
//    [("1910", 9203, 0.001856504301); 

//6) See how the name looks over the years using a chart
#load "../packages/FSharp.Charting.0.90.13/FSharp.Charting.fsx"
open FSharp.Charting
//
//val chartData : seq<string * int>
//val it : ChartTypes.GenericChart = (Chart)

//7) Calculate basic statiistics for a name - Min.Max,Mean
//
//val it : string = "10"
//> 
//val it : float = 7174.0
//> 
//val it : float = 704.4430865

//8) Calculate the attachment point and determine the popular years for a name
//
//val variance : values:seq<float> -> float
//val standardDeviation : values:seq<float> -> float
//val attachmentPoint : values:seq<float> -> float
//val popularYears : name:string -> ((string * int) * (string * int)) option
//val it : ((string * int) * (string * int)) option =
//  Some (("1942", 77419), ("1964", 73344))

//9) Instead of year of birth, can the state of birth tell us anything?
//Create a chart of names by US State for a given name
//
//val chartData' : seq<string * int>
//val it : ChartTypes.GenericChart = (Chart)

//10) Detemine what % of names are in top Quartile of states
//
//val topQuartileStates : seq<string * int * float>
//val topQuartileTotal : int = 2755952
//val total : int = 4957166
//val it : float = 0.5559531394

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
//
//type AgeGroup =
//  | Old
//  | MiddleAged
//  | Young
//type Gender =
//  | Male
//  | Female
//val getNameCategory : name:string -> AgeGroup option * Gender option
//val it : AgeGroup option * Gender option = (Some MiddleAged, Some Male)

//12) Once you have the age/gender result for a name
//return it as a tuple with the name and a string description
//For example
//Some Old, Some Male = {Name} , Old Male'
//
//val getNamePrintout : name:string -> string * string

//13) Write to disk
//
//- Interrupt








