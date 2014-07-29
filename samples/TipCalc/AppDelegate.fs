namespace TipCalc

open System
open MonoTouch.UIKit
open MonoTouch.Foundation
open Xamarin.Forms
open Xamarin.Forms.Xaml

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()

    let mutable window: UIWindow = null

    override x.FinishedLaunching (app, options) =
        Forms.Init ()
        window <- new UIWindow (UIScreen.MainScreen.Bounds)

        let model = TipCalcModel(TipPercent = "15")
        let view = TipCalcView()
        let controller = TipCalcController()
        let mvc = Mvc(model, view, controller)
        let eventLoop = mvc.Start()

        window.RootViewController <- view.Root.CreateViewController ()
        window.MakeKeyAndVisible ()
        true

module Main =
    [<EntryPoint>]
    let main args = 
        UIApplication.Main (args, null, "AppDelegate")
        0

