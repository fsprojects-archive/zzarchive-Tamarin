namespace TipCalc

open System
open Xamarin.Forms

open Tamarin

type TipCalcModel() =
    inherit Model()

    let mutable subTotal: string = null 
    let mutable postTaxTotal: string = null 
    let mutable tipPercent: string = null 
    let mutable tipAmount = Nullable<decimal>()
    let mutable total = Nullable<decimal>()

    member this.SubTotal with get() = subTotal and set value = subTotal <- value; this.NotifyPropertyChanged <@ this.SubTotal @>
    member this.PostTaxTotal with get() = postTaxTotal and set value = postTaxTotal <- value; this.NotifyPropertyChanged <@ this.PostTaxTotal @>
    member this.TipPercent with get() = tipPercent and set value = tipPercent <- value; this.NotifyPropertyChanged <@ this.TipPercent @>
    member this.TipAmount with get() = tipAmount and set value = tipAmount <- value; this.NotifyPropertyChanged <@ this.TipAmount @>
    member this.Total with get() = total and set value = total <- value; this.NotifyPropertyChanged <@ this.Total @>

type TipCalcEvents = Calculate

type TipCalcView() as this = 
    inherit View<TipCalcEvents, TipCalcModel, ContentPage>(root = ContentPage())    

    do
        this.Root.LoadFromXamlResource "TipCalcPage.xaml"

    let subTotal: Entry = this.Root ? SubTotal
    let postTaxTotal: Entry = this.Root ? PostTaxTotal
    let tipPercent: Entry = this.Root ? TipPercent
    let tipPercentSlider: Slider = this.Root ? TipPercentSlider
    let tipAmount: Label = this.Root ? TipAmount
    let total: Label = this.Root ? Total
        
    override this.EventStreams = 
        [
            subTotal.TextChanged |> Observable.mapTo Calculate
            postTaxTotal.TextChanged |> Observable.mapTo Calculate
            tipPercent.TextChanged |> Observable.mapTo Calculate
        ] 

    override this.SetBindings model = 
        
        Binding.OfExpression 
            <@ 
                subTotal.Text <- model.SubTotal 
                postTaxTotal.Text <- model.PostTaxTotal
                tipPercent.Text <- model.TipPercent

                tipAmount.Text <- String.Format("{0:C}", model.TipAmount)
                total.Text <- String.Format("{0:C}", model.Total)
            @>

        let converter = IValueConverter.Create(Double.Parse, Math.Round >> string)       

        Binding.OfExpression(
            <@ 
                tipPercentSlider.Value <- converter.Apply model.TipPercent
            @>,
            BindingMode.TwoWay)

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
