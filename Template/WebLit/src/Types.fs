namespace Lit.TodoMVC

open System

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
