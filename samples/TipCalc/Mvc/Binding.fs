[<RequireQualifiedAccess>]
module Binding

open System
open System.Reflection
open Xamarin.Forms
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

type PropertyInfo with
    member this.DependencyProperty = 
        this.DeclaringType
            .GetField(this.Name + "Property", BindingFlags.Static ||| BindingFlags.Public)
            .GetValue(null, [||]) 
            |> unbox<BindableProperty> 

let (|Target|_|) = function 
    | Some( FieldGet( Some( Value( window, _)), control)) -> 
        window |> control.GetValue |> unbox<BindableObject> |> Some
    | _ -> None

let rec (|Source|_|) = function 
    | PropertyGet( Some( Value _), sourceProperty, []) -> 
        Some( Binding(path = sourceProperty.Name))
    | NewObject( ctorInfo, [ Source binding ] ) 
        when ctorInfo.DeclaringType.GetGenericTypeDefinition() = typedefof<Nullable<_>> -> 
        Some binding 
    | SpecificCall <@ String.Format : string * obj -> string @> (None, [], [ Value(:? string as format, _); Coerce( Source binding, _) ]) ->
        binding.StringFormat <- format
        Some binding
    | Call(None, ``method``, [ Source binding ]) -> 
        binding.Mode <- BindingMode.OneWay
        binding.Converter <- {
            new IValueConverter with
                member this.Convert(value, _, _, _) = ``method``.Invoke(null, [| value |])
                member this.ConvertBack(_, _, _, _) = raise <| NotImplementedException()
        }
        Some binding
    | _ -> None

let rec split = function 
    | Sequential(head, tail) -> head :: split tail
    | tail -> [ tail ]

let ofExpression expr = 
    for e in split expr do
        match e with 
        | PropertySet(Target target, targetProperty, [], Source binding) ->
            target.SetBinding(targetProperty.DependencyProperty, binding) |> ignore
        | expr -> failwithf "Invalid binding quotation:\n%O" expr

