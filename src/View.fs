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
    type Layout<'T when 'T :> View> with

        member this.AddChildren([<ParamArray>] children) = 
            children |> Array.iter this.Children.Add

        member this.Children 
            with set value =
                this.Children.Clear()
                value |> Array.iter this.Children.Add

[<AbstractClass>]
type View<'Event, 'Model, 'Root when 'Root :> Element>(root: 'Root) = 

    member this.Root = root

    interface IView<'Event, 'Model> with
        member this.Events = 
            this.EventStreams.Merge() 
        member this.SetBindings model = 
            this.SetBindings model
            root.BindingContext <- model

    abstract EventStreams : IObservable<'Event> list
    abstract SetBindings : 'Model -> unit
