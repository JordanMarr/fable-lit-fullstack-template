module WebLit.Todos

let private hmr = Lit.HMR.createToken()

// Dummy trigger so the module can be imported if not loaded directly from a <script> tag
// (e.g. from the tests) and the web components registered
let register() = ()

module Elmish =
    open System
    open Elmish

    type Todo =
        { Id: Guid
          Description: string
          Completed: bool }
        static member New(description: string) =
            { Id = Guid.NewGuid()
              Description = description
              Completed = false }

    type State =
        { Todos: Todo list
          Edit: Todo option
          Sort: bool }

    type Msg =
        | ToggleSort
        | AddNewTodo of description: string
        | DeleteTodo of Guid
        | ToggleCompleted of Guid
        | StartEdit of Todo
        | FinishEdit of Todo option

    let init() =
        let todos = [ Todo.New("Learn F#"); Todo.New("Have fun with Lit!") ]
        { Todos = todos; Edit = None; Sort = false }, Cmd.none

    let update msg model =
        match msg with
        | ToggleSort ->
            { model with Sort = not model.Sort }, Cmd.none

        | AddNewTodo description ->
            let todo = Todo.New(description)
            { model with Todos = todo::model.Todos ; Edit = None }, Cmd.none

        | DeleteTodo guid ->
            let todos = model.Todos |> List.filter (fun t -> t.Id <> guid)
            { model with Todos = todos }, Cmd.none

        | ToggleCompleted guid ->
            let todos = model.Todos |> List.map (fun t ->
                if t.Id = guid then { t with Completed = not t.Completed } else t)
            { model with Todos = todos }, Cmd.none

        | StartEdit edit  ->
            { model with Edit = Some edit }, Cmd.none

        | FinishEdit None ->
            { model with Edit = None }, Cmd.none

        | FinishEdit(Some t1) ->
            let todos = model.Todos |> List.map (fun t2 ->
                if t1.Id = t2.Id then t1 else t2)
            { model with Todos = todos; Edit = None }, Cmd.none

open Util
open Elmish
open Browser.Types
open Lit

[<HookComponent>]
let NewTodoEl dispatch =
    Hook.useHmr(hmr)
    let inputRef = Hook.useRef<HTMLInputElement>()
    let addNewTodo _ =
        match inputRef.Value with
        | None -> ()
        | Some input ->
            let value = input.value
            input.value <- ""
            match value.Trim() with
            | "" -> ()
            | v -> v |> AddNewTodo |> dispatch

    html $"""
        <div class="field has-addons">
            <div class="control is-expanded">
                <input class="input is-medium"
                    type="text"
                    aria-label="New todo description"
                    {Lit.refValue inputRef}
                    @keyup={Ev(onEnterOrEscape addNewTodo ignore)}>
            </div>
            <div class="control">
                <button class="button is-primary is-medium"
                    aria-label="Add new todo"
                    @click={Ev addNewTodo}>
                    <i role="img" class="bi-plus-lg"></i>
                </button>
            </div>
        </div>
    """

[<HookComponent>]
let TodoEl dispatch (edit: Todo option) (todo: Todo) =
    Hook.useHmr(hmr)
    let transitionMs = 500
    let className = Hook.use_scoped_css $"""
        :host {{
            transition-duration: {transitionMs}ms;
            border: 2px solid lightgray;
            border-radius: 10px;
            margin: 5px 0;
        }}
        :host.transition-enter {{
            opacity: 0;
            transform: scale(2);
        }}
        :host.transition-leave {{
            opacity: 0;
            transform: scale(0.1);
        }}
        .is-clickable {{
            user-select: none;
        }}
    """

    let isEditing =
        match edit with
        | Some edit -> edit.Id = todo.Id
        | None -> false

    let hasFocus = Hook.useRef(false)
    let inputRef = Hook.useRef<HTMLInputElement>()

    Hook.useEffectOnChange(isEditing, function
        | true when not hasFocus.Value ->
            inputRef.Value.Iter(fun i -> i.select())
        | _ -> ())

    let transition = Hook.useTransition(transitionMs, onLeft = (fun () -> DeleteTodo todo.Id |> dispatch))

    let inner =
        if isEditing then
            let applyEdit _ =
                inputRef.Value.Iter(fun input ->
                    { todo with Description = input.value.Trim() }
                    |> Some
                    |> FinishEdit
                    |> dispatch)

            let cancelEdit _ =
                FinishEdit None |> dispatch

            html $"""
            <div class="column is-10">
                <input class="input"
                    type="text"
                    aria-label="Edit todo"
                    {Lit.refValue inputRef}
                    value={todo.Description}
                    @keyup={Ev(onEnterOrEscape applyEdit cancelEdit)}
                    @blur={Ev cancelEdit}>
            </div>
            <div class="column is-2">
                <button class="button is-primary" aria-label="Save edit"
                    @click={Ev applyEdit}>
                    <i role="img" class="bi-save"></i>
                </button>
            </div>
            """
        else
            html $"""
            <div class="column is-9">
                <p class="subtitle is-clickable"
                    @dblclick={Ev(fun _ -> StartEdit todo |> dispatch)}>
                    {todo.Description}
                </p>
            </div>
            <div class="column is-3">
                <button class={Lit.classes ["button", true; "is-success", todo.Completed]}
                    aria-label={if todo.Completed then "Mark uncompleted" else "Mark completed"}
                    @click={Ev(fun _ -> ToggleCompleted todo.Id |> dispatch)}>
                    <i role="img" class="bi-check-lg"></i>
                </button>
                <button class="button is-primary" aria-label="Edit"
                    @click={Ev(fun _ -> StartEdit todo |> dispatch)}>
                    <i role="img" class="bi-pencil"></i>
                </button>
                <button class="button is-danger" aria-label="Delete"
                    @click={Ev(fun _ -> transition.triggerLeave())}>
                    <i role="img" class="bi-trash"></i>
                </button>
            </div>
            """

    html $"""<div class="columns {className} {transition.className}" {LitLabs.motion.animate()}>{inner}</div>"""

[<LitElement("todo-app")>]
let TodoApp() =
    Hook.useHmr(hmr)
    let _, props = LitElement.init(fun cfg ->
        // We need a LitElement to use @lit-labs/motion/animate
        // But we don't use Shadow DOM so we can use global CSS rules
        cfg.useShadowDom <- false
        cfg.props <-
            {|
                localStorage = Prop.Of(None, attribute="local-storage")
            |}
    )

    // Scoped CSS can be used together with global rules (like Bulma, Bootstrap, etc)
    let className = Hook.use_scoped_css """
        :host {
            margin: 0 auto;
            max-width: 800px;
            padding: 20px;
        }
    """

    let model, dispatch =
        Hook.useElmishWithLocalStorage(init, update, ?storageKey=props.localStorage.Value)

    let todos =
        if not model.Sort then model.Todos
        else model.Todos |> List.sortBy (fun t -> t.Description.ToLower())

    html $"""
    <div class={className}>
        <div class="title">
            <slot name="title"></slot>
        </div>

        {NewTodoEl dispatch}

        <label class="checkbox">
            <input type="checkbox"
                ?checked={model.Sort}
                @change={Ev(fun _ -> dispatch ToggleSort)}>
            Sort by description
        </label>

        {todos |> Lit.mapUnique
            (fun t -> string t.Id)
            (TodoEl dispatch model.Edit)}
    </div>
    """
