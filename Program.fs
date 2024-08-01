open System.Net.Http
open FSharp.Data

// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

["Ryan";"Ben";"M"]
|> List.filter (fun x -> (x.Length > 3))
|> printfn "%A"

let client = new HttpClient()
let ip = client.GetStringAsync("https://ifconfig.me/ip").Result

printfn $"{ip}"

(* VALUE TYPES & BINDING *)
// values are bound to a name using the `let` keyword
// value types are inferred but can be explicitely annotated

// String value with explicit type annotation
let greeting : string = "Hello"

// Boolean value
let win = true

// Integer value
let points = 42

// Tuple
let coordinates = (1.0, 1.5)

// List
let phonetics = ["Alpha"; "Bravo"; "Charlie"]

// Lambda expressions
let double_nums =
    [1;2;3;4;5]
    |> List.map (fun x -> x * 2)

printfn $"{double_nums}"

// A Lambda expresssion is an anonymouse function
// but if we bind it to a name it becomes a normal function we can call
let double x = x * 2

printfn $"{double 50}"

(* UPDATING VALUES *)
// by default all values are immutable, to allow us to update a value we must
// mark it as `mutable`
let mutable destination = "Paris"

// we can then update the value with the `<-` operator
destination <- "London"

(* FUNCTIONS *)
// Lambda expressions
// F#'s equivalent to `void` is Unit `()`, we use this where a Lambda does not
// return a value
// fun () -> printfn "Hello"
// fun x -> x + 1
// fun x y -> x + y

// Named functions
// Named functions are like Lambda expressions but they let us hide away some of
// the complexity. For example, the following named function is equivalent to:
// let increment = fun x -> x + 1
let increment x = x + 1
printfn $"{increment 9}"

// If a named function returns Unit we don't need to specify this with `()`.
// let print_greeting = fun () -> printfn "Hi!"
let print_greeting = printfn "Hi!"

(* TYPE ANNOTATIONS *)
// The compiler will attempt to infer the types of our values, but when dealing
// with function parameters specificity can be important:
let url_builder proto base_url path =
    $"{proto}://{base_url}/{path}"

// This is fine where we know what types are needed to use the function correctly:
printfn $"""{url_builder "https" "github.com" "microsoft/fsharp"}"""
// But this may not always be the case:
printfn $"""{url_builder 1 "github.com" (1.0, 2.0)}"""
// The function still works but it produces junk output that's useless to us.
// `1://github.com/(1, 2)` isn't a valid URL we can use!

// To guard against this we can use explicit type annotation in our functions to
// define what types our parameters and return type should be:
let better_url_builder (proto : string) (base_url : string) (path : string) : string =
    $"{proto}://{base_url}/{path}"

// Our new function work identically to the old one:
printfn $"""{better_url_builder "https" "github.com" "microsoft/fsharp"}"""
// But this will flag an error in our editor and the compiler:
// printfn $"""{better_url_builder 1 "github.com" (1.0, 2.0)}"""

// Option type
// The option type is like null in other languages, where a function may produce
// `Some` data or it may produce `None`, to do this we must annotate our return
// value as an `option`: 
let get_html (html_file : string) : HtmlDocument option =
    try
        let html = HtmlDocument.Load(html_file)
        Some(html)
    with ex ->
        printfn $"error: {ex}"
        None

// Calling HtmlDocument.Load directly with an invalid uri argument would
// cause an error:
// let my_html_file = HtmlDocument.Load "nofile"

// our `get_html` function would still error, but it returns `None` when it does
// which we can handle far more gracefully:
// let my_html_file = get_html "no_file"

(* PIPELINES *)
