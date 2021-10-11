module Index

open Elmish
open Fable.Remoting.Client
open Shared
open System

type Model = { Todos: Todo list; Input: string; ShowCompleted: bool; }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo list
    | CompleteTodo of Guid
    | CompletedTodo of Todo list
    | ToggleCompletedDisplay

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

let init () : Model * Cmd<Msg> =
    let model = { Todos = []; Input = ""; ShowCompleted=false }

    let cmd =
        Cmd.OfAsync.perform todosApi.getTodos () GotTodos

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | GotTodos todos -> { model with Todos = todos }, Cmd.none
    | SetInput value -> { model with Input = value }, Cmd.none
    | AddTodo ->
        let todo = Todo.create model.Input

        let cmd =
            Cmd.OfAsync.perform todosApi.addTodo todo AddedTodo

        { model with Input = "" }, cmd
    | AddedTodo todos ->
        { model with
              Todos = todos  },
        Cmd.none
    | CompleteTodo givenId -> 
        let cmd = Cmd.OfAsync.perform todosApi.completeTodo givenId CompletedTodo
        model, cmd
    | CompletedTodo todos ->
        {model with Todos = todos } , Cmd.none
    | ToggleCompletedDisplay -> { model with ShowCompleted = not model.ShowCompleted}, Cmd.none

open Feliz
open Feliz.Bulma

let navBrand =
    Bulma.navbarBrand.div [
        Bulma.navbarItem.a [
            prop.href "https://safe-stack.github.io/"
            navbarItem.isActive
            prop.children [
                Html.img [
                    prop.src "/favicon.png"
                    prop.alt "Logo"
                ]
            ]
        ]
    ]

let completeTodo (todo: Todo) (dispatch: Msg -> unit) = 
    Html.li [
        Bulma.field.div [
            prop.children [
                 Bulma.field.div [
                    prop.text todo.Description
                ]
            ]
        ]
    ]


let incompleteTodo (todo: Todo) (dispatch: Msg -> unit) = 
    Html.li [
        Bulma.field.div [
            prop.children [
                Bulma.field.div [
                    prop.text todo.Description
                ]
                Bulma.button.a [
                    color.isLight
                    prop.onClick (fun _ -> CompleteTodo todo.Id |> dispatch)
                    prop.text "Complete?"
                ]
            ]
        ]
    ]

let todosToDisplay model = 
    if model.ShowCompleted then
        model.Todos
    else
        model.Todos |> List.filter(fun e -> not e.Completed)

let containerBox (model: Model) (dispatch: Msg -> unit) =
    let displayables = todosToDisplay model    

    Bulma.box [
        Bulma.content [
            Html.ol [
                if displayables.IsEmpty then
                    Bulma.field.div [
                        prop.text "All Caught up!"
                    ]
                else
                    for todo in displayables do
                        if(todo.Completed) then
                            completeTodo todo dispatch
                        else
                            incompleteTodo todo dispatch
            ]
        ]
        Bulma.field.div [
            field.isGrouped
            prop.children [
                Bulma.control.p [
                    control.isExpanded
                    prop.children [
                        Bulma.input.text [
                            prop.value model.Input
                            prop.placeholder "What needs to be done?"
                            prop.onChange (fun x -> SetInput x |> dispatch)
                        ]
                    ]
                ]
                Bulma.control.p [
                    Bulma.button.a [
                        color.isPrimary
                        prop.disabled (Todo.isValid model.Input |> not)
                        prop.onClick (fun _ -> dispatch AddTodo)
                        prop.text "Add"
                    ]
                ]
            ]
        ]
    ]

let showHideToggle (model: Model) (dispatch: Msg -> unit) = 
    let verb = if(model.ShowCompleted)then "Hide " else "Show " 
    let buttonText =  verb +  "Completed Items"

    Bulma.button.a[
        prop.text buttonText
        prop.onClick (fun _ -> ToggleCompletedDisplay |> dispatch)
    ]


let view (model: Model) (dispatch: Msg -> unit) =
    Bulma.hero [
        hero.isFullHeight
        color.isPrimary
        prop.style [
            style.backgroundSize "cover"
            style.backgroundImageUrl "https://unsplash.it/1200/900?random"
            style.backgroundPosition "no-repeat center center fixed"
        ]
        prop.children [
            Bulma.heroHead [
                Bulma.navbar [
                    Bulma.container [ navBrand ]
                ]
            ]
            Bulma.heroBody [
                Bulma.container [
                    Bulma.column [
                        column.is6
                        column.isOffset3
                        prop.children [
                            Bulma.title [
                                text.hasTextCentered
                                prop.text "fantastic_enigma"
                            ]
                            showHideToggle model dispatch
                            containerBox model dispatch
                        ]
                    ]
                ]
            ]
        ]
    ]
