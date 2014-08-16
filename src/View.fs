namespace Tamarin

open System
open System.Reflection
open Xamarin.Forms
open System.Reactive.Linq

[<RequireQualifiedAccess>]
module Observable =
    let mapTo value = Observable.map(fun _ -> value)

[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Extensions = 

    open System.Collections.Generic
    type IList<'T> with
        member this.AddRange([<ParamArray>] items) = 
            items |> Array.iter this.Add
           
    let (?) (this : #VisualElement) name = 
        match this.FindByName name with
        | null -> invalidArg "Name" ("Cannot find element with name: " + name)
        | control -> unbox control 

[<AbstractClass>]
type View<'Event, 'Model, 'Root when 'Root :> VisualElement>(root: 'Root) = 

    member this.Root = root

    interface IView<'Event, 'Model> with
        member this.Events = 
            this.EventStreams.Merge() 
        member this.SetBindings model = 
            this.SetBindings model
            root.BindingContext <- model
        member this.Navigation = 
            root.Navigation

    abstract EventStreams : IObservable<'Event> list
    abstract SetBindings : 'Model -> unit
