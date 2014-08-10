namespace TabbedPageDemo

open System
open MonoTouch.UIKit
open MonoTouch.Foundation
open Xamarin.Forms

open Tamarin

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()

    let mutable window: UIWindow = null

    override this.FinishedLaunching (app, options) =
        Forms.Init ()
        window <- new UIWindow (UIScreen.MainScreen.Bounds)

        let view = TabbedPageDemoView()
//        view.Root.BindingContext <- MonkeyDataModel.All
//        view.Root.SetBinding( TabbedPage.ItemsSourceProperty, Binding())
        (view :> IView<_, _>).SetBindings( MonkeyDataModel.All)
        window.RootViewController <- view.Root.CreateViewController ()
        window.MakeKeyAndVisible ()
        true

module Main =
    [<EntryPoint>]
    let main args = 
        UIApplication.Main (args, null, "AppDelegate")
        0

