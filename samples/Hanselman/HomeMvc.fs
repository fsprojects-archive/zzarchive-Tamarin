namespace Hanselman

open Xamarin.Forms
open Tamarin

type HomeModel() = 
    inherit Model()

type HomeView() as this = 
    inherit View<unit, HomeModel, MasterDetailPage>(root = MasterDetailPage())

    let master = HomeMasterView()
    do
        //this.Root.Master <- master.Root
        this.Root.Icon <- downcast FileImageSource.FromFile( "slideout.png")

    override this.EventStreams = 
        []
    override this.SetBindings model = 
        ()

and HomeMasterModel() = 
    inherit Model()

and HomeMasterView() as this = 
    inherit View<unit, HomeModel, ContentPage>(root = ContentPage())

    let root = this.Root
    do
        root.Icon <- downcast FileImageSource.FromFile( "slideout.png") 

        let layout = StackLayout( Spacing = 0.)

        let label = 
            ContentView(
                Padding = new Thickness(10., 36., 0., 5.),
                BackgroundColor = Color.Transparent,
                Content = Label(
                    Text = "MENU",
                    Font = Font.SystemFontOfSize (NamedSize.Medium)
                )
            )

        layout.Children.Add( label) 

        root.Content <- layout

    override this.EventStreams = 
        []

    override this.SetBindings model = 
        ()
        
     