﻿namespace ChickenCheck.Domain

open ChickenCheck.Domain
open ChickenCheck.Domain.Events
open System

type ConnectionString = ConnectionString of string
type AppendEvents = DomainEvent list -> AsyncResult<unit list, DatabaseError>


module Store =
    module User =
        type GetUserByEmail = Email -> AsyncResult<User option, DatabaseError>
    module Chicken =
        type GetChickens = unit -> AsyncResult<Chicken list, DatabaseError>
        type GetTotalEggCount = unit -> AsyncResult<Map<ChickenId, EggCount>, DatabaseError>
        type GetEggCountOnDate = Date -> AsyncResult<Map<ChickenId, EggCount>, DatabaseError>
        type AddEgg = ChickenId * Date -> AsyncResult<unit, DatabaseError>
        type RemoveEgg = ChickenId * Date -> AsyncResult<unit, DatabaseError>
        type GetEggData = unit -> AsyncResult<Map<ChickenId, Map<Date, NaturalNum>>, DatabaseError>
