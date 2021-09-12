namespace AcctShared

open System
open Events

module Account =
    let CreditAcctEventKind = Events.createKind ("Credit")

    let DebitAcctEventKind = Events.createKind ("Debit")

    type CreditAcctEventDetails = { Amount: float }
    type DebitAcctEventDetails = { Amount: float }

    let deposit (amount: float) =
        let eventDetails = { Amount = amount }

        { Id = Guid.NewGuid()
          Kind = CreditAcctEventKind
          Details = eventDetails.ToString() }

    let credit (amount: float) =
        let eventDetails = { Amount = amount }

        { Id = Guid.NewGuid()
          Kind = DebitAcctEventKind
          Details = eventDetails.ToString() }

type IAccountApi =
    { getBalance: unit -> Async<float>
      deposit: float -> Async<Result<unit, string>>
      withdraw: float -> Async<Result<unit, string>> }
