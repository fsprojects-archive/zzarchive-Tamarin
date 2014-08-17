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

type TabbedPageItem() as this = 
    inherit ContentPage()
    do
        this.LoadFromXamlResource "TabbedPageDemoPage.xaml"

    member this.Name : Label = this ? Name
    member this.Picture : Image = this ? Picture
    member this.Family : Label = this ? Family
    member this.SubfamilySection : StackLayout = this ? SubfamilySection
    member this.Subfamily: Label = this ? Subfamily
    member this.TribeSection : StackLayout = this ? TribeSection
    member this.Tribe: Label = this ? Tribe
    member this.Genus: Label = this ? Genus


type TabbedPageDemoView() as this = 
    inherit StaticView<MonkeyDataModel[], TabbedPage>(root = TabbedPage())    

    do
        this.Root.ItemTemplate <- DataTemplate(typeof<TabbedPageItem>)

    override this.SetBindings model = 
        
        this.Root.SetBindings(
            itemsSource = <@ model @>, 
            itemBindings = fun (itemTemplate : TabbedPageItem) model ->
                <@@
                    itemTemplate.Title <- model.Name
                    itemTemplate.Name.Text <- model.Name
                    itemTemplate.Picture.Source <- coerce model.PhotoUrl
                    itemTemplate.Family.Text <- model.Family
                    itemTemplate.SubfamilySection.IsVisible <- model.Subfamily <> null && model.Subfamily <> ""
                    itemTemplate.Subfamily.Text <- model.Subfamily
                    itemTemplate.TribeSection.IsVisible <- not( String.IsNullOrEmpty( model.Tribe))
                    itemTemplate.Tribe.Text <- model.Tribe
                    itemTemplate.Genus.Text <- model.Genus
                @@>
        )
             
    
