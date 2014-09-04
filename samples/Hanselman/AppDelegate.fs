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
        window <- new UIWindow (UIScreen.MainScreen.Bounds)
        UINavigationBar.Appearance.SetTitleTextAttributes( UITextAttributes( TextColor = UIColor.White))
    
        Forms.Init ()
        
        let view = HomeMasterView()
        window.RootViewController <- view.Root.CreateViewController ()
        window.MakeKeyAndVisible ()
        true

module Main =
    [<EntryPoint>]
    let main args = 
        UIApplication.Main (args, null, "AppDelegate")
        0

