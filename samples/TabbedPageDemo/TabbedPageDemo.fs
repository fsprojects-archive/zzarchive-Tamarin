namespace TabbedPageDemo

open System
open Xamarin.Forms

type MonkeyDataModel = {
    Name : string
    Family : string
    Subfamily : string
    Tribe : string
    Genus : string
    PhotoUrl : string
}   with

    static member All = 
        [|
            {   
                Name = "Chimpanzee"
                Family = "Hominidae"
                Subfamily = "Homininae"
                Tribe = "Panini"
                Genus = "Pan"
                PhotoUrl="http://upload.wikimedia.org/wikipedia/commons/thumb/6/62/Schimpanse_Zoo_Leipzig.jpg/640px-Schimpanse_Zoo_Leipzig.jpg" 
            }
            {   
                Name = "Orangutan"
                Family = "Hominidae"
                Subfamily = "Ponginae"
                Tribe = null
                Genus = "Pongo"
                PhotoUrl = "http://upload.wikimedia.org/wikipedia/commons/b/be/Orang_Utan%2C_Semenggok_Forest_Reserve%2C_Sarawak%2C_Borneo%2C_Malaysia.JPG"
            }
            {
                Name = "Tamarin"
                Family = "Callitrichidae"
                Subfamily = null
                Tribe = null
                Genus = "Saguinus"
                PhotoUrl = "http://upload.wikimedia.org/wikipedia/commons/thumb/8/85/Tamarin_portrait_2_edit3.jpg/640px-Tamarin_portrait_2_edit3.jpg"
            }
        |] 

open Tamarin

type NonNullToBooleanConverter() = 
    interface IValueConverter with
        member __.Convert( value, _, _, _) =
            box( string value <> "")
        member __.ConvertBack (_, _, _, _) = null

type TabbedPageDemoView() as this = 
    inherit View<unit, MonkeyDataModel[], TabbedPage>(root = TabbedPage())    

    do
        this.Root.LoadFromXamlResource "TabbedPageDemoPage.xaml"

//    let name : Label = this.Root ? Name
//    let picture: Image = this.Root ? Picture
//    let family: Label = this.Root ? Family
//    let subfamilySection : StackLayout = this.Root ? SubfamilySection
//    let subfamily: Label = this.Root ? Subfamily
//    let tribeSection : StackLayout = this.Root ? TribeSection
//    let tribe: Label = this.Root ? Tribe
//    let genus: Label = this.Root ? Genus
        
    override this.EventStreams = []

    override this.SetBindings model = 
        
        Binding.OfExpression 
            <@
                this.Root.ItemsSource <- model
            @>

        this.Root.CurrentPageChanged.Add( fun args ->
            let item = this.Root.ItemTemplate
            //let name : Label = this.Root ? Name
            ()
        )
        
    
