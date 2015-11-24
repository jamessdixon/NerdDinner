namespace NerdDinner

open System.Linq
open System.Linq.Expressions



type public IRepository<'T> =
   abstract member All : unit -> IQueryable<'T>
   abstract member AllIncluding: unit -> IQueryable<'T>
   abstract member Find: int -> 'T
   abstract member InsertOrUpdate: 'T -> unit
   abstract member Delete: int -> unit
   abstract member SubmitChanges unit -> unit

type public IDinnerRepository =
   interface IRepository<Dinner> with
       abstract member Print : unit -> unit
