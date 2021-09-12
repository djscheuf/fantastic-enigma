namespace AuthN

open System

type PasswordHash: {value: string} // Probably some salted hash of user's password, used for authentication.

type Username : {Valeue: string} // Simple string storage of users, with some rules applied to constrain that string.

type User: {Id: Guid; Name: Username; Password: PasswordHash}