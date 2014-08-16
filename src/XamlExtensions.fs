[<AutoOpen>]
module Tamarin.XamlExtensions 

open System.Reflection
open System.IO
open Xamarin.Forms

let internal loadFromXaml = 
    typeof<Xaml.Extensions>.GetRuntimeMethods() |> Seq.find (fun x -> x.Name = "LoadFromXaml" && not x.IsPublic)

let internal getCallingAssemblyHandler = 
    typeof<Assembly>.GetRuntimeMethod( "GetCallingAssembly", Array.empty)

type VisualElement with
    member this.LoadFromXamlResource(embeddedResourse : string) =
        let callingAssembly = getCallingAssemblyHandler.Invoke( null, Array.empty) |> unbox<Assembly>
        use stream = callingAssembly.GetManifestResourceStream( embeddedResourse)
        use reader = new StreamReader( stream)
        let xaml = reader.ReadToEnd()
        loadFromXaml.MakeGenericMethod(this.GetType()).Invoke(null, [| this; xaml |]) |> ignore
           
  