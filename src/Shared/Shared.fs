namespace Shared

open System

type Todo =
    { Id: Guid
      Description: string
      Completed: bool }

module Todo =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) =
        { Id = Guid.NewGuid()
          Description = description
          Completed = false }

    let complete (todo: Todo) = 
        if(todo.Completed) then
            todo
        else 
            { todo with Completed = true; Description = "Completed: "+ todo.Description }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ITodosApi =
    { getTodos: unit -> Async<Todo list>
      addTodo: Todo -> Async<Todo>
      completeTodo: Guid -> Async<string> }
