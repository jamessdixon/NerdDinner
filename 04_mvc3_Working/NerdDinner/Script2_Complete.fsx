#r "System.Data.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"
#r "../packages/WindowsAzure.Storage.4.3.0/lib/net40/Microsoft.WindowsAzure.Storage.dll"

open System
open System.Data
open System.Text
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

type schema = SqlDataConnection< @"data source=DIXON13J\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=NerdDinner;MultipleActiveResultSets=True">
let context = schema.GetDataContext()

let dinnerData = query { for dinner in context.Dinners do
                            join rsvp in context.RSVPs on (dinner.DinnerID = rsvp.DinnerID)
                            select (dinner,rsvp)}
                            |> Seq.map(fun (dinner,rsvp) -> dinner.HostedById.Trim(), dinner.EventDate.DayOfWeek, rsvp.RsvpID)
                            |> Seq.groupBy(fun (h,d,r) -> h,d)
                            |> Seq.map(fun (hd,a) -> hd,a |> Seq.countBy(fun(h,d,r)->r))
                            |> Seq.map(fun (hd,a) -> hd,a |> Seq.sumBy(fun(i,c)->c))
                            |> Seq.map(fun ((h,d),c) -> h,d.ToString(),c.ToString())
                            |> Seq.map(fun (h,d,c) -> h + "," + d + "," + c + "\n")
                            |> Seq.map(fun s -> Encoding.Default.GetBytes(s))
                            |> Seq.collect(fun a -> a)
                            |> Seq.toArray

open System.IO
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Blob

let containerName = "nerddinner"
let connectionString =
    let stringBuilder = new StringBuilder()
    stringBuilder.Append("DefaultEndpointsProtocol=http;") |> ignore
    stringBuilder.Append("AccountName=") |> ignore
    stringBuilder.Append("chickenstorage") |> ignore
    stringBuilder.Append(";") |> ignore
    stringBuilder.Append("AccountKey=") |> ignore
    stringBuilder.Append("cmOIeDMzQsykeQ533B5cKLZVm06a4YkwHb2NlJ95Hq9bfOSGC/ePZX/og6byr+mSGQy+fj0ONW1QWi2vBZmo9Q==") |> ignore
    stringBuilder.Append(";") |> ignore
    stringBuilder.ToString()

let getBlobContainer(blobClient:Blob.CloudBlobClient) =
    let container = blobClient.GetContainerReference(containerName)
    if not (container.Exists()) then
        container.CreateIfNotExists() |> ignore
        let permissions = new BlobContainerPermissions()
        permissions.PublicAccess <- BlobContainerPublicAccessType.Blob
        container.SetPermissions(permissions)
    container

let getBlockBlob() =
    let storageAccount = CloudStorageAccount.Parse(connectionString)
    let blobClient = storageAccount.CreateCloudBlobClient()
    let container = getBlobContainer(blobClient) 
    container.GetBlockBlobReference("nerdData.csv") 

let insertDinners(dinnerData:byte[]) =
    let blockBlob = getBlockBlob() 
    use memoryStream = new MemoryStream(dinnerData)
    blockBlob.UploadFromStream(memoryStream)

insertDinners dinnerData

