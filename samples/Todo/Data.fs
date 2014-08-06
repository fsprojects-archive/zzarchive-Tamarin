namespace Todo

open SQLite.Net
open SQLite.Net.Attributes
open Xamarin.Forms

[<CLIMutable>]
type TodoItem = {
    [<PrimaryKey; AutoIncrement>]
    mutable ID: int
    mutable Name: string
    mutable Done: bool
    mutable Notes: string
}      

type Database(conn: SQLiteConnection) = 

    let guard = obj()

    do
        conn.CreateTable<TodoItem>() |> ignore

    member this.GetItems() = 
        lock guard <| fun() -> 
            conn.Table<TodoItem>()

    member this.GetItem id = 
        lock guard <| fun() -> 
            conn.Find<TodoItem>(fun x -> x.ID = id)

    member this.SaveItem(item: TodoItem) = 
        lock guard <| fun() -> 
            conn.InsertOrReplace item

    member this.DeleteItem(item: TodoItem) = 
        lock guard <| fun() -> 
            conn.Delete item
