module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open System
open System.IO

open Shared


// type Storage() =
//     //let todos = Todo list

//     member __.GetTodos() = List.ofSeq todos

//     member __.AddTodo(todo: Todo) =
//         if Todo.isValid todo.Description then
//             todos.Add todo
//             Ok()
//         else
//             Error "Invalid todo"
    
//     member __.CompleteTodo(givenId: Guid) =
//         if todos.Exists(fun e -> e.Id = givenId) then 
//             let todoIndex = todos.FindIndex(fun e -> e.Id = givenId)
//             let completedTodo = todos.Find(fun e -> e.Id = givenId) |> Todo.complete
//             todos.RemoveAt todoIndex
//             todos.Insert(todoIndex, completedTodo)
//             Ok()
//         else 
//             Error "Todo does not Exist"

// let storage = Storage()

// storage.AddTodo(Todo.create "Create new SAFE project")
// |> ignore

// storage.AddTodo(Todo.create "Write your app")
// |> ignore

// storage.AddTodo(Todo.create "Ship it !!!")
// |> ignore

// storage.AddTodo(Todo.create "Whip it !!!")
// |> ignore

let storagePath = "./Data/db.csv";
//[ ] Next time -> Write a Func that save myTodos into a JSON Doc.
// JCJ Usually using a directory and some files to that DIR. 

let getTodos path = 
    async{
        if File.Exists(path)
        then 
            let! lines = File.ReadAllLinesAsync(path) |> Async.AwaitTask
            return  lines
            |> Array.toList
            |> List.map (Todo.recreate)
        else
            printfn "File Does Not Exist"
            return []
    }

let readTodos = getTodos storagePath

let writeTodos contents = File.WriteAllLinesAsync(storagePath, contents)

let storeTodos existingTodos = 
    async {
        let dehydrated = 
            existingTodos 
            |> List.map (Todo.dehydrate)

        dehydrated
        |> printfn "%A"

        writeTodos dehydrated
        |> Async.AwaitTask
        |> ignore

        return! readTodos
    }

type Todos = list<Todo>
//TODO eventually replace that empty list with fun that reads from disk.

let addTodos existingTodos newTodo =
    newTodo::existingTodos 
    |> storeTodos

let completeTodo existingTodos givenId = 
    existingTodos 
    |> List.map (fun e -> if e.Id = givenId then Todo.complete e else e)
    |> storeTodos



let todosApi =
    { 
      getTodos = 
        fun () -> async { return! readTodos } 
      addTodo =
          fun todo -> 
          async {
              let! existingTodos = readTodos;
              return! addTodos existingTodos todo 
            }
              
      completeTodo = 
        fun givenId -> async {
            let! existingTodos = readTodos;
            return! completeTodo existingTodos givenId 
        }
   }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
