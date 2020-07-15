namespace ChickenCheck.UnitTests

open System
open System.IO
open System.Reflection
open ChickenCheck.Backend
open ChickenCheck.Shared
open Microsoft.Data.Sqlite
open SimpleMigrations
open SimpleMigrations.DatabaseProvider

type TestDb(?file) =
    do SQLitePCL.Batteries_V2.Init()
    let file = file |> Option.defaultValue (Guid.NewGuid().ToString("N") + ".db")
    let migrateDb connectionString =
        let migrationAssembly = Assembly.GetAssembly(typeof<ChickenCheck.Migrations.CreateUserTable>)
        let connection = new SqliteConnection(connectionString)
        connection.Open()
        let provider = SqliteDatabaseProvider(connection)
        let migrator = SimpleMigrator(migrationAssembly, provider)
        migrator.Load()
        migrator.MigrateToLatest()
        connection
        
    let connectionString = sprintf "Data Source=%s;" file
    let connection = migrateDb connectionString
    do OptionHandler.register()
    
    let chickenStore = Database.ChickenStore (Database.ConnectionString.create connectionString)
    
    member this.ChickenStore = chickenStore
    interface IDisposable with
        member this.Dispose() =
            connection.Close()
            File.Delete file

module DbChickens =
    let bjork = {| Id = ChickenId.parse "b65e8809-06dd-4338-a55e-418837072c0f" 
                   Name = "Bjork" |}
                    
    let bodil = {| Id = ChickenId.parse "dedc5301-b404-49f4-8d5c-7bfac10c6950" 
                   Name = "Bodil" |}
                   
    let siouxsie = {| Id = ChickenId.parse "309671cc-b23d-46db-bf3c-545a95d3f949" 
                      Name = "Siouxsie Sioux" |}
                   
    let malin = {| Id = ChickenId.parse "5287cec6-feb8-49c2-aef1-2dfe4d822366" 
                   Name = "Malin" |}
                      
    let lina = {| Id = ChickenId.parse "ed5366d0-70f7-4e54-84a2-1281a23dafb6" 
                  Name = "Lina" |}

    let vivianne = {| Id = ChickenId.parse "0103b370-3714-4c44-b5a9-a83dd0231b53" 
                      Name = "viviainne" |}

