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
    
    let recreate (driedTodo: string) =
        let parts = driedTodo.Split [|','|]
        { 
            Id = Guid.Parse(parts.[0])
            Description = parts.[1]
            Completed = Boolean.Parse(parts.[2])
        }

    let dehydrate theTodo = 
        let driedV = $"%s{theTodo.Id.ToString()}, \"%s{theTodo.Description}\", %s{theTodo.Completed.ToString()}"
        printfn "%s" driedV
        driedV

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ITodosApi =
    { getTodos: unit -> Async<Todo list>
      addTodo: Todo -> Async<Todo list>
      completeTodo: Guid -> Async<Todo list> }
