namespace Todo

open SQLite.Net
open SQLite.Net.Attributes
open Xamarin.Forms

[<CLIMutable>]
type TodoItem = {
    [<PrimaryKey; AutoIncrement>]
    ID: int
    Name: string
    Done: bool
    Notes: string
}   with

    static member Create(name, ?``done``, ?notes) = {
        ID = 0
        Name = name
        Done = defaultArg ``done`` false
        Notes = defaultArg notes ""
    }
       

type Database(conn: SQLiteConnection) = 

    let guard = obj()

    do
        conn.CreateTable<TodoItem>() |> ignore

    member this.GetItems() = 
        lock guard <| fun() -> 
            conn.Table<TodoItem>() |> Seq.toArray  

    member this.GetItemsNotDone() = 
        lock guard <| fun() -> 
            conn.Query<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0")

    member this.GetItem id = 
        lock guard <| fun() -> 
            conn.Find<TodoItem>(fun x -> x.ID = id)

    member this.SaveItem(item: TodoItem) = 
        lock guard <| fun() -> 
            conn.InsertOrReplace item

    member this.DeleteItem(item: TodoItem) = 
        lock guard <| fun() -> 
            conn.Delete item
