[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Tamarin.Binding

open System
open System.Reflection
open Xamarin.Forms
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.ExprShape

let inline private undefined<'T> = raise<'T> <| NotImplementedException()

type IValueConverter with 
    static member Create(convert : 'a -> 'b, convertBack : 'b -> 'a) =  {
        new IValueConverter with
            member this.Convert(value, targetType, _, _) = 
                assert (typeof<'a> = targetType)
                value |> unbox |> convert |> box 
            member this.ConvertBack(value, targetType, _, _) = 
                assert (typeof<'b> = targetType)
                value |> unbox |> convertBack |> box 
    }
    static member OneWay convert = IValueConverter.Create(convert, fun _ -> undefined)

    member this.Apply<'Source, 'Target>(sourceProperty : 'Source) : 'Target = undefined

module internal Patterns = 

    type PropertyInfo with
        member this.BindableProperty = 
            this.DeclaringType
                .GetField(this.Name + "Property", BindingFlags.Static ||| BindingFlags.Public)
                .GetValue(null, [||]) 
                |> unbox<BindableProperty> 

    let (|Target|_|) expr = 
        let rec loop = function
            | Some( Value(obj, viewType) ) -> obj
            | Some( FieldGet(tail, field) ) ->  field.GetValue(loop tail)
            | Some( PropertyGet(tail, prop, []) ) -> prop.GetValue(loop tail, [||])
            | _ -> null
        match loop expr with
        | :? BindableObject as target -> Some target
        | _ -> None

    let (|PropertyPath|_|) expr = 
        let rec loop e acc = 
            match e with
            | PropertyGet( Some tail, property, []) -> 
                loop tail (property.Name :: acc)
            | Value _ | Var _ -> Some acc
            | _ -> None

        loop expr [] |> Option.map (String.concat "." )

    let (|StringFormat|_|) = function
        | SpecificCall <@ String.Format : string * obj -> string @> (None, [], [ Value( :? string as format, _); Coerce( propertyPath, _) ]) ->
            Some(format, propertyPath)
        | _ -> None    

    let (|Converter|_|) = function
        | Call(instance, method', [ propertyPath ]) -> 
            let instance = match instance with | Some( Value( value, _)) -> value | _ -> null
            Some((fun(value : obj) -> method'.Invoke(instance, [| value |])), propertyPath )
        | _ -> None    
         
    let private getUnboxImpl = 
        let ref = Type.GetType("Microsoft.FSharp.Core.Operators, FSharp.Core").GetMethod("Unbox")
        fun t -> ref.MakeGenericMethod [| t |]

    let private getBoxImpl = 
        let ref = Type.GetType("Microsoft.FSharp.Core.Operators, FSharp.Core").GetMethod("Box")
        fun t -> ref.MakeGenericMethod [| t |]

    type PropertyInfo with
        member internal this.IsNullableValue =  
            this.DeclaringType.IsGenericType && this.DeclaringType.GetGenericTypeDefinition() = typedefof<Nullable<_>> && this.Name = "Value"

    let rec (|SourceProperty|_|) = function 
        | PropertyGet( Some _, prop, []) -> Some prop 
        | PropertyGet( Some( PropertyGet( Some _, step1, [])), step2, []) when step2.IsNullableValue -> Some step1 
        | _ -> None

    let rec extractPropertyGetters propertyBody = 
        seq {
            match propertyBody with 
            | PropertyGet _ as getter -> yield getter
            | ShapeVar _ -> ()
            | ShapeLambda( _, body) -> yield! extractPropertyGetters body   
            | ShapeCombination( _, exprs) -> for subExpr in exprs do yield! extractPropertyGetters subExpr
        }

    let (|SinglePropertyExpression|_|) expr = 
        match expr |> extractPropertyGetters |> Seq.toList with
        | [ SourceProperty prop as getterToReplace ] ->
            let propertyValue = Var("value", typeof<obj>)
            let rec replacePropertyWithParam expr = 
                match expr with 
                | PropertyGet _ as getter when getter = getterToReplace -> 
                    Expr.Call( getUnboxImpl prop.PropertyType, [ Expr.Var propertyValue ])
                | ShapeVar var -> Expr.Var( var)
                | ShapeLambda( var, body) -> Expr.Lambda(var, replacePropertyWithParam body)  
                | ShapeCombination( shape, exprs) -> ExprShape.RebuildShapeCombination( shape, exprs |> List.map( fun e -> replacePropertyWithParam e))

            let converterBody = Expr.Call(getBoxImpl expr.Type, [ replacePropertyWithParam expr ])
            let converter: obj -> obj = 
                Expr.Lambda(propertyValue, converterBody)
                |> Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation 
                |> unbox

            Some( Binding(prop.Name, BindingMode.OneWay, converter = IValueConverter.OneWay converter))
        | _ -> None

    let rec (|Source|) = function
        | PropertyPath path -> 
            Binding(path) 
        | Coerce( Source binding, _) ->
            binding
        | StringFormat(format, Source binding) -> 
            binding.StringFormat <- format
            binding
        | Call(None, ``method``, [ Value(:? IValueConverter as converter, _); Source binding ] ) when ``method``.Name = "IValueConverter.Apply" -> 
            binding.Converter <- converter
            binding
        | Converter(convert, Source binding) -> 
            binding.Mode <- BindingMode.OneWay
            binding.Converter <- IValueConverter.OneWay convert
            binding
        | SinglePropertyExpression binding -> 
            binding
        | expr -> 
            invalidArg "binding property path quotation" (string expr)

open Patterns

type Binding with
    static member OfExpression(expr, ?mode) =
        let rec split = function 
            | Sequential(head, tail) -> head :: split tail
            | tail -> [ tail ]

        for e in split expr do
            match e with
            | PropertySet(Target target, targetProperty, [], Source binding) ->
                mode |> Option.iter binding.set_Mode 
                target.SetBinding(targetProperty.BindableProperty, binding)
            | _ -> invalidArg "expr" (string e) 

type ListView with
    member this.SetBindings(itemsSource : Expr<#seq<'Item>>, selectedItem : Expr<'Item>) = 
        
        this.SetBinding(ListView.ItemsSourceProperty, (|Source|) itemsSource)
        this.SetBinding(ListView.SelectedItemProperty, (|Source|) selectedItem)
 