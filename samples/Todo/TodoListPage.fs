namespace Todo

open System
open Xamarin.Forms

open Tamarin

type TodoItemCell() as this = 
    inherit ViewCell()

    let label = Label(YAlign = TextAlignment.Center)

    let tick = Image(Source = FileImageSource.FromFile ("check.png"))

    let layout = 
        StackLayout(
            Padding = new Thickness(20., 0., 0., 0.),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.StartAndExpand
        )
    do
        layout.AddChildren( label, tick) 
        this.View <- layout

    override this.OnBindingContextChanged () = 
        // Fixme : this is happening because the View.Parent is getting 
        // set after the Cell gets the binding context set on it. Then it is inheriting
        // the parents binding context.
        this.View.BindingContext <- this.BindingContext
        base.OnBindingContextChanged ()

type TodoListPage() as this = 
    inherit ContentPage()

    let listView = ListView( RowHeight = 40)
    do
        this.Title <- "Todo"
        NavigationPage.SetHasNavigationBar (this, true)

        listView.ItemTemplate <- DataTemplate typeof<TodoItemCell>
        
        // HACK: workaround issue #894 for now
        if (Device.OS = TargetPlatform.iOS)
        then
            listView.ItemsSource <- [| "" |]

        let layout = StackLayout()
        if (Device.OS = TargetPlatform.WinPhone)  // WinPhone doesn't have the title showing
        then
            layout.Children.Add <| Label( Text="Todo", Font = Font.SystemFontOfSize(NamedSize.Large))
        layout.Children.Add(listView)
        layout.VerticalOptions <- LayoutOptions.FillAndExpand
        this.Content <- layout

        let tbi =
            let activated = Action(fun() -> this.Navigation.PushAsync( TodoItemPage( BindingContext = Activator.CreateInstance<TodoItem>())) |> ignore)

            match Device.OS with
            | TargetPlatform.iOS -> ToolbarItem("+", null, activated, enum 0, 0)
            | TargetPlatform.Android -> ToolbarItem ("+", "plus", activated, enum 0, 0)
            | TargetPlatform.WinPhone -> ToolbarItem("Add", "add.png", activated, enum 0, 0)
            | _ -> null

        this.ToolbarItems.Add (tbi)

        if Device.OS = TargetPlatform.iOS
        then
            let activated = Action(fun() -> ())
            let tbi2 = ToolbarItem("?", null, activated, enum 0, 0)
            this.ToolbarItems.Add (tbi2)

    override this.OnAppearing () = 
        base.OnAppearing ()
        //listView.ItemsSource <- App.Database.GetItems ()

