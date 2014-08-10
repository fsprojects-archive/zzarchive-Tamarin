[<AutoOpen>]
module Tamarin.XamlExtensions 

open System.Reflection
open System.IO

open MonoTouch.Foundation
open Xamarin.Forms

let internal loadFromXaml = typeof<Xaml.Extensions>.GetMethod("LoadFromXaml", BindingFlags.NonPublic ||| BindingFlags.Static)

type VisualElement with
    member this.LoadFromXamlResource(bundledResourse : string) =
        let file = Path.Combine(NSBundle.MainBundle.BundlePath, bundledResourse)
        assert (File.Exists file)
        let xaml = File.ReadAllText file
        loadFromXaml.MakeGenericMethod(this.GetType()).Invoke(null, [| this; xaml |]) |> ignore
           
let (?) (this : #VisualElement) name = 
    match this.FindByName name with
    | null -> invalidArg "Name" ("Cannot find element with name: " + name)
    | control -> unbox control 

     