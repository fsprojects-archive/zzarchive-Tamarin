namespace Tamarin

[<AbstractClass>]
type Controller<'Event, 'Model>() =
    interface IController<'Event, 'Model> with
        member this.InitModel model = this.InitModel model
        member this.Dispatcher = this.Dispatcher

    abstract InitModel : 'Model -> unit
    abstract Dispatcher : ('Event -> EventHandler<'Model>)

    static member Create callback = {
        new IController<'Event, 'Model> with
            member __.InitModel _ = () 
            member __.Dispatcher = Sync << callback
    }


