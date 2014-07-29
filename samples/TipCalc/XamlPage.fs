namespace global

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

[<AutoOpen>]
module Extensions = 

    type IValueConverter with 
        static member Create(convert : 'a -> 'b, convertBack : 'b -> 'a) =  {
            new IValueConverter with
                member this.Convert(value, _, _, _) = 
                    try 
                        value |> unbox |> convert |> box 
                    with why -> 
                        null
                member this.ConvertBack(value, _, _, _) = 
                    try 
                        let typedValue = unbox value
                        let result = convertBack typedValue
                        box result
                        //value |> unbox |> convertBack |> box 
                    with why -> 
                        null
        }
        static member OneWay convert = IValueConverter.Create(convert, fun _ -> raise <| NotImplementedException())
