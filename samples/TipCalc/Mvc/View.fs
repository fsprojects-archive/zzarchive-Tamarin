namespace global

open System.Reflection
open System.IO
open System
open Xamarin.Forms

[<RequireQualifiedAccess>]
module Observable =
    let mapTo value = Observable.map(fun _ -> value)

    let internal empty<'a> = {
        new IObservable<'a> with
            member __.Subscribe _ = {
                new IDisposable with 
                    member __.Dispose() = ()
            }
    } 

[<AbstractClass>]
type View<'Event, 'Model, 'Root when 'Root :> Element>(root: 'Root) = 

    member this.Root = root

    interface IView<'Event, 'Model> with
        member this.Events = 
            this.EventStreams |> List.fold Observable.merge Observable.empty
        member this.SetBindings model = 
            this.SetBindings model
            root.BindingContext <- model

    abstract EventStreams : IObservable<'Event> list
    abstract SetBindings : 'Model -> unit
