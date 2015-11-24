namespace NerdDinner.Models

open System
open System.Linq
open System.Linq.Expressions

type IRepository<'T> =
  abstract All : IQueryable<'T>
  abstract AllIncluding 
    : [<ParamArray>] includeProperties:Expression<Func<'T, obj>>[] -> IQueryable<'T>
    abstract member Find: int -> 'T
    abstract member InsertOrUpdate: 'T -> unit
    abstract member Delete: int -> unit
    abstract member SubmitChanges: unit -> unit

type IDinnerRepository =
  inherit IRepository<Dinner>
  abstract member FindByLocation: float*float -> IQueryable<Dinner>
  abstract FindUpcomingDinners : unit -> IQueryable<Dinner>
  abstract FindDinnersByText : string -> IQueryable<Dinner>
  abstract member DeleteRsvp: 'T -> unit



