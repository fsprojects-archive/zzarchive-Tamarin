namespace Tamarin

open System
open System.ComponentModel

type IView<'Event, 'Model> = 
    abstract Events : IObservable<'Event> with get
    abstract SetBindings : 'Model -> unit

type EventHandler<'Model> = 
    | Sync of ('Model -> unit)
    | Async of ('Model -> Async<unit>)

type IController<'Event, 'Model> =
    abstract InitModel : 'Model -> unit
    abstract Dispatcher : ('Event -> EventHandler<'Model>)

[<Sealed>]
type Mvc<'Event, 'Model when 'Model :> INotifyPropertyChanged>(model : 'Model, view : IView<'Event, 'Model>, controller : IController<'Event, 'Model>) =

    member this.Start() =
        controller.InitModel model
        view.SetBindings model

        view.Events.Subscribe( fun event -> 
            match controller.Dispatcher event with
            | Sync eventHandler -> eventHandler model
            | Async eventHandler -> Async.StartImmediate( eventHandler model)
        )
