namespace Events

open System

type EventKind = {Id: Guid; Name: string}

type Event = {Id: Guid; Kind: EventKind; Details: string ;} 

module Events = 
    let createKind(name: string) = 
        {
            Id = Guid.NewGuid()
            Name = name 
        }

