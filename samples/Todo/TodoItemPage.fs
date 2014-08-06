namespace Todo

open System.Collections.Generic
open Xamarin.Forms
open Tamarin

type TodoItemEvents = 
    | Save
    | Delete
    | Cancel
    | Speak

type TodoItemView() as this = 
    inherit View<TodoItemEvents, TodoItem, ContentPage>(root = ContentPage())

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

    do
        NavigationPage.SetHasNavigationBar (this.Root, true)

        this.Root.Content <- 
            let layout = StackLayout(VerticalOptions = LayoutOptions.StartAndExpand, Padding = Thickness 20.)
            layout.Children.AddRange(
                nameLabel, nameEntry, 
                notesLabel, notesEntry,
                doneLabel, doneEntry,
                saveButton, deleteButton, cancelButton,
                speakButton
            )

            layout


    override this.SetBindings model = 
        Binding.OfExpression
            <@
                this.Root.Title <- model.Name
                nameEntry.Text <- model.Name
                notesEntry.Text <- model.Notes
                doneEntry.IsToggled <- model.Done
            @>

    override this.EventStreams = 
        [
            saveButton, Save
            deleteButton, Delete
            cancelButton, Cancel
            speakButton, Speak
        ]
        |> List.map ( fun (button, event) -> button.Clicked |> Observable.mapTo event)

type TodoItemController( conn, textToSpeech: ITextToSpeech) =
    inherit Controller<TodoItemEvents, TodoItem>()

    let database = Database( conn)

    override this.InitModel _ = ()

    override this.Dispatcher = function
        | Save -> Sync( database.SaveItem >> ignore)
        | Delete -> Sync( database.DeleteItem >> ignore)
        | Cancel -> Async this.Cancel
        | Speak -> Sync this.Speak 

    member this.Cancel _ =
        this.Navigation.PopAsync() |> Async.AwaitTask |> Async.Ignore 

    member this.Speak model =
        textToSpeech.Speak( model.Name + " " + model.Notes)

