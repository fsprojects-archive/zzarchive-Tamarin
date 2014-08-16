namespace Hanselman

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

//        let view = TabbedPageDemoView()
//        let model = MonkeyDataModel.All
//        view.Root.BindingContext <- model
//        view.SetBindings model

        let view = ContentPage()
        window.RootViewController <- view.CreateViewController ()
        window.MakeKeyAndVisible ()
        true

module Main =
    [<EntryPoint>]
    let main args = 
        UIApplication.Main (args, null, "AppDelegate")
        0

