(* MODULES *)
// Modulkes are used to organise code into related namespaces
// top level module representing the file
module Modules 
module Domain =

    type MAddress = { HouseNumber: int; StreetName: string }
    type MPhoneNumber = { Code: int; Number: string }

    type ContactMethod =
        | MPostalMail of MAddress
        | MEmail of string
        | MVoiceMail of MPhoneNumber
        | SMS of MPhoneNumber

module Messenger =

    // to access the types and union in the Domain module we `open` it
    open Domain

    let msend_message (message: string) (method: ContactMethod) =
        match method with
        | MPostalMail { HouseNumber=h; StreetName=s } -> printfn $"Posting: {message} to {h} {s}"
        | MEmail a -> printfn $"Emailing: {message} to {a}"
        | MVoiceMail { Code=c; Number=n } -> printfn $"Leaving +{c} {n} a voicemail saying: {message}"
        | SMS { Code=c; Number=n } -> printfn $"SMS messaging +{c} {n}: {message}"