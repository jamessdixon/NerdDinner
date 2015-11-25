
#r "../packages/FSharp.Data.2.2.5/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data


//1) Bring in the data from disk (Data folder)
//Create 1 large dataframe with all of the state data

//2) Sum up names regardless of state or year

//3) Sum up all records in the USA

//4) Most popular names?

//5) Make a function to see how a name splits by gender

//6) Make a function to see how a name splits by year of birth

//7) Combine the functions using a high order function

//8) See how the name looks over the years using a chart
#load "../packages/FSharp.Charting.0.90.13/FSharp.Charting.fsx"
open FSharp.Charting

//9) Using the attachment point, determine the popular years for a name

//10) Instead of year of birth, can the state of birth tell us anything?

//11) Quartiles

//12) So we have somthing with age, but not really state
//Let's create a lookup file to be used in our application
//Assume any name whose gender is > 75% is should be catagorized as that gender
//For example, 'James' is 99% male, so it should be catagorized as 'Male'
//Also, assume anyone whose last year as a popular name was before 1945 is old
//And anyone whose last popular year is after 1980 is young

//13) Write to disk









