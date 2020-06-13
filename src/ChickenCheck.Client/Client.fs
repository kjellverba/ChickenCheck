module Client

open ChickenCheck.Shared
open Elmish
open Elmish.React
open ChickenCheck.Client
open CompositionRoot
open Feliz
open Feliz.Router
open ChickenCheck.Client.Chickens

module Routing =
    let handleUrlChange nextUrl model =
        
        let show page = { model with CurrentPage = page; CurrentUrl = nextUrl }
        let chickensPage date = show (Page.Chickens (ChickensPageModel.init date))
        let getAllChickens date = GetAllChickens (Start date) |> ChickenMsg |> Cmd.ofMsg
        
        match nextUrl with
        | Url.Home -> model, Router.navigate "/chickens"
        | Url.Chickens date -> chickensPage date, getAllChickens date
        | Url.NotFound -> show Page.NotFound, Cmd.none
        | Url.LogIn destination ->
            // auth log in
            match destination with
            | Some d ->
                model, Router.navigate d
            | None ->
                model, Router.navigate "/chickens"
        | Url.LogOut ->
            // auth log out
                { model with User = Anonymous }, Router.navigate "/chickens"
            

// defines the initial state and initial command (= side-effect) of the application
let private init () =
    let initialUrl = parseUrl (Router.currentUrl())
    let defaultModel =
        { Settings = HasNotStartedYet
          User = Anonymous
          CurrentUrl = initialUrl
          CurrentPage = Page.Chickens (ChickensPageModel.init NotFutureDate.today)
          IsMenuExpanded = false }
          
    let goToChickens date =
        { defaultModel with
            CurrentPage = Page.Chickens (ChickensPageModel.init date) }, GetAllChickens (Start date) |> ChickenMsg |> Cmd.ofMsg
    match initialUrl with
    | Url.Home -> goToChickens NotFutureDate.today
    | Url.Chickens date ->
        goToChickens date
    | Url.NotFound ->
        { defaultModel with CurrentPage = Page.NotFound }, Cmd.none
    | Url.LogIn _ ->
        // auth log in
//        Auth0.authLock.show()
        defaultModel, Cmd.none
    | Url.LogOut ->
        // auth log out
        goToChickens NotFutureDate.today
    

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg, model.CurrentPage with
    | UrlChanged nextUrl, _ -> 
        Routing.handleUrlChange nextUrl model
        
    | ChickenMsg msg, Page.Chickens pageModel ->
        let (pageModel, cmds) = Chickens.Update.update chickenCmds msg pageModel
        { model with CurrentPage = Page.Chickens pageModel }, cmds |> Cmd.map ChickenMsg

    | ToggleMenu, _ ->
        { model with IsMenuExpanded = not model.IsMenuExpanded }, Cmd.none
        
    | Logout,_ ->
        notImplemented()
        model, Cmd.none
        
    | msg, page -> 
        sprintf "Unhandled msg: %A, page: %A" msg page |> Utils.Log.developmentError
        model, Cmd.none

let view (model: Model) dispatch =
    let activePage =
        match model.CurrentPage with
        | Page.Chickens pageModel -> Chickens.View.view {| Model = pageModel; Dispatch = ChickenMsg >> dispatch; User = model.User |}
        | Page.NotFound -> lazyView NotFound.view model

    let toggleMenu() = dispatch ToggleMenu
    Router.router [
        Router.onUrlChanged (parseUrl >> UrlChanged >> dispatch)
        Router.application [
            Html.div [
                Navbar.view {| User = model.User; IsMenuExpanded = model.IsMenuExpanded; ToggleMenu = toggleMenu |}
                Html.div [ activePage ]
            ]
        ]
    ]


#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
#endif

|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif

|> Program.run
