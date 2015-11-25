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


