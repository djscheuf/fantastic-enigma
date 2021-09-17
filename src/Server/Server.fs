module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open System

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

type Todos = list<Todo>
//TODO eventually replace that empty list with fun that reads from disk.
let getTodos: Todos = [];
let addTodos existingTodos newTodo =
    newTodo::existingTodos
let completeTodo existingTodos givenId = 
    existingTodos 
    |> List.map (fun e -> if e.Id = givenId then Todo.complete e else e)


//[ ] Next time -> Write a Func that save myTodos into a JSON Doc.
// JCJ Usually using a directory and some files to that DIR. 

let todosApi =
    { getTodos = fun () -> async { return getTodos }
      addTodo =
          fun todo ->
              async { return addTodos (getTodos) todo }
      completeTodo = 
        fun givenId -> 
          async { return completeTodo (getTodos) givenId } 
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
