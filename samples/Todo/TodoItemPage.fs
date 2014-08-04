namespace Todo

open System.Collections.Generic
open Xamarin.Forms
open Tamarin

type TodoItemPage() as this = 
    inherit ContentPage()

    do
        NavigationPage.SetHasNavigationBar (this, true)
        let nameLabel = Label( Text = "Name" )
        let nameEntry = Entry ()

        let notesLabel = Label( Text = "Notes" )
        let notesEntry = Entry ()

        let doneLabel = Label( Text = "Done")
        let doneEntry = Switch ()

        let saveButton = Button( Text = "Save")
        let deleteButton = Button( Text = "Delete" )
        let cancelButton = Button( Text = "Cancel")
        let speakButton = Button( Text = "Speak")

        this.Content <- 
            let layout = StackLayout(VerticalOptions = LayoutOptions.StartAndExpand, Padding = Thickness 20.)
            layout.Children.AddRange(
                nameLabel, nameEntry, 
                notesLabel, notesEntry,
                doneLabel, doneEntry,
                saveButton, deleteButton, cancelButton,
                speakButton
            )

            layout

type TodoItemModel() =
    inherit Model() 

    let mutable items = Array.empty<TodoItem>
    let mutable selectedTask = Unchecked.defaultof<TodoItem>

    member this.Items 
        with get() = items
        and set value = 
            items <- value
            this.NotifyPropertyChanged <@ this.Items @>

    member this.SelectedTask 
        with get() = selectedTask
        and set value = 
            selectedTask <- value
            this.NotifyPropertyChanged <@ this.SelectedTask @>
