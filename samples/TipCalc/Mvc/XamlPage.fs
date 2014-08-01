namespace Tamarin

open System.Reflection
open System.IO
open System

open MonoTouch.Foundation
open Xamarin.Forms

type XamlPage(bundledResourse: string) as this =
    inherit ContentPage() 

    static let loadFromXaml = typeof<Xaml.Extensions>.GetMethod("LoadFromXaml", BindingFlags.NonPublic ||| BindingFlags.Static)

    do
        let file = Path.Combine(NSBundle.MainBundle.BundlePath, bundledResourse)
        assert (File.Exists file)
        let xaml = File.ReadAllText file
        loadFromXaml.MakeGenericMethod(typeof<XamlPage>).Invoke(null, [| this; xaml |]) |> ignore

    static member (?) (this: XamlPage, name) = 
        match this.FindByName name with
        | null -> invalidArg "Name" ("Cannot find element with name: " + name)
        | control -> unbox control 

