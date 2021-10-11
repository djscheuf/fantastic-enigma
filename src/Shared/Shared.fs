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
        let fullDesc = parts.[1].Replace("%2C",",")
        { 
            Id = Guid.Parse(parts.[0])
            Description = fullDesc
            Completed = Boolean.Parse(parts.[2])
        }

    let dehydrate theTodo = 
        let safeDesc = theTodo.Description.Replace(",","%2C")
        let driedV = $"%s{theTodo.Id.ToString()}, %s{safeDesc}, %s{theTodo.Completed.ToString()}"
        printfn "%s" driedV
        driedV

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ITodosApi =
    { getTodos: unit -> Async<Todo list>
      addTodo: Todo -> Async<Todo list>
      completeTodo: Guid -> Async<Todo list> }
