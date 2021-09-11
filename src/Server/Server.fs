module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open System

open Shared

type Storage() =
    let todos = ResizeArray<_>()

    member __.GetTodos() = List.ofSeq todos

    member __.AddTodo(todo: Todo) =
        if Todo.isValid todo.Description then
            todos.Add todo
            Ok()
        else
            Error "Invalid todo"
    
    member __.CompleteTodo(givenId: Guid) =
        if todos.Exists(fun e -> e.Id = givenId) then 
            let todoIndex = todos.FindIndex(fun e -> e.Id = givenId)
            let completedTodo = todos.Find(fun e -> e.Id = givenId) |> Todo.complete
            todos.RemoveAt todoIndex
            todos.Insert(todoIndex, completedTodo)
            Ok()
        else 
            Error "Todo does not Exist"

let storage = Storage()

storage.AddTodo(Todo.create "Create new SAFE project")
|> ignore

storage.AddTodo(Todo.create "Write your app")
|> ignore

storage.AddTodo(Todo.create "Ship it !!!")
|> ignore

storage.AddTodo(Todo.create "Whip it !!!")
|> ignore

let todosApi =
    { getTodos = fun () -> async { return storage.GetTodos() }
      addTodo =
          fun todo ->
              async {
                  match storage.AddTodo todo with
                  | Ok () -> return todo
                  | Error e -> return failwith e
              }
      completeTodo = 
        fun givenId -> 
          async { 
              match storage.CompleteTodo givenId with
              | Ok () -> return "Completed"
              | Error e -> return failwith e } 
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
