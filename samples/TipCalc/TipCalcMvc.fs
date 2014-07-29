namespace global

open System
open Xamarin.Forms

type TipCalcModel() =
    inherit Model()

    let mutable subTotal: string = null 
    let mutable postTaxTotal: string = null 
    let mutable tipPercent: string = null 
    let mutable tipAmount = Nullable<decimal>()
    let mutable total = Nullable<decimal>()

    member this.SubTotal with get() = subTotal and set value = subTotal <- value; this.NotifyPropertyChanged "subTotal"
    member this.PostTaxTotal with get() = postTaxTotal and set value = postTaxTotal <- value; this.NotifyPropertyChanged "PostTaxTotal"
    member this.TipPercent with get() = tipPercent and set value = tipPercent <- value; this.NotifyPropertyChanged "TipPercent"
    member this.TipAmount with get() = tipAmount and set value = tipAmount <- value; this.NotifyPropertyChanged "TipAmount"
    member this.Total with get() = total and set value = total <- value; this.NotifyPropertyChanged "Total"

type TipCalcEvents = Calculate

type TipCalcView() = 

    let page = XamlPage("TipCalcPage.xaml")
    let subTotal: Entry = page ? SubTotal
    let postTaxTotal: Entry = page ? PostTaxTotal
    let tipPercent: Entry = page ? TipPercent
    let tipPercentSlider: Slider = page ? TipPercentSlider
    let tipAmount: Label = page ? TipAmount
    let total: Label = page ? Total
        
    member this.Root = page

    interface IView<TipCalcEvents, TipCalcModel> with

        member this.Events = 
            [
                subTotal.TextChanged :?> IObservable<_>
                upcast postTaxTotal.TextChanged 
                upcast tipPercent.TextChanged
            ] 
            |> List.reduce Observable.merge
            |> Observable.map (fun _ -> Calculate)

        member this.SetBindings model = 
            
            subTotal.SetBinding(Entry.TextProperty, "SubTotal")
            postTaxTotal.SetBinding(Entry.TextProperty, "PostTaxTotal")
            tipPercent.SetBinding(Entry.TextProperty, "TipPercent")
            tipPercentSlider.SetBinding(
                Slider.ValueProperty, 
                "TipPercent", 
                BindingMode.TwoWay, 
                converter = IValueConverter.Create(Decimal.Parse >> Decimal.ToDouble, fun x -> Decimal(x) |> Decimal.Round |> string)
            )
            tipAmount.SetBinding(Label.TextProperty, "TipAmount", stringFormat = "{0:C}")
            total.SetBinding(Label.TextProperty, "Total", stringFormat = "{0:C}")

            page.BindingContext <- model
            
type TipCalcController() = 
    inherit Controller<TipCalcEvents, TipCalcModel>()

    override this.InitModel model =
        model.TipPercent <- string 15

    override this.Dispatcher = function 
        | Calculate -> Sync this.Calculate 

    member this.Calculate model = 
        match Decimal.TryParse( model.TipPercent), Decimal.TryParse( model.SubTotal) with
        | (true, tipPercent), (true, subTotal) -> 
            model.TipAmount <- Nullable(Math.Round (tipPercent * subTotal / 100M, 2))
            let success, postTaxTotal = Decimal.TryParse( model.PostTaxTotal)
            if success 
            then 
                model.Total <- Nullable(Math.Round(4M * (postTaxTotal + model.TipAmount.Value)) / 4M)
        | _ -> 
            model.TipAmount <- Nullable()
            model.Total <- Nullable()
