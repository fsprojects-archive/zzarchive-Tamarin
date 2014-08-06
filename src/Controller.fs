namespace Tamarin

[<AbstractClass>]
type Controller<'Event, 'Model>() =

    let mutable navigation = null
    
    interface IController<'Event, 'Model> with
        member this.InitModel model = this.InitModel model
        member this.Dispatcher = this.Dispatcher
        member this.Navigation with set value = navigation <- value

    abstract InitModel : 'Model -> unit
    abstract Dispatcher : ('Event -> EventHandler<'Model>)

    member this.Navigation = navigation

    static member Create callback = {
        new Controller<'Event, 'Model>() with
            member __.InitModel _ = () 
            member __.Dispatcher = Sync << callback
    }


