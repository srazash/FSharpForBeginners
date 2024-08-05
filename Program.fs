open System
open System.Net.Http
open System.IO
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
// We can chain functions together using pipelines, this allows us to compose
// compaund functions from other smaller functions
let get_links (html : HtmlDocument option) =
    // pattern matching over option type to get 'a' tags
    match html with
    | Some (x) -> x.Descendants [ "a" ]
    | None -> Seq.empty

// We could simply use one function as the argument to another
// get_links (get_html "no_file")

let html_doc = "https://www.antibubble.org/index.html"

// But F# allows us to pipeline between values and functions
html_doc
    |> get_html
    |> get_links
    |> printfn "%A"
// Here we have a freestanding String that represents a HTML document, we then
// pipeline this as the input of get_html, we pipeline get_html's output as the
// input of get_links, and finally we pipeline get_links' output to printfn.

// However, we are still simply pipelining the input and outputs of functions
// we can compose entirely new functions with the composition operator `>>`:
let links_from_html =
    get_html >> get_links

// Now we can use this new compound function as a single function on it's own or
// within a pipeline:
html_doc
    |> links_from_html
    |> Seq.iter (fun x -> printfn $"{x}")

(* TUPLES, RECORDS & DISCRIMINATED UNIONS *)
// Tuples allow us to group unnamed values of different types in order.
let point = (1.0, 2.0)
let github_stars = ("dotnet/fsharp", 2800)

// We can access the first and second items in a tuple using the `fst` and `snd`
// keywords:
printfn $"The GitHub repo '{fst github_stars}' has {snd github_stars} stars"

// Tuple values can be bound to value names:
let repo_name, repo_stars = github_stars
// If we only wanted a single value from the tuple we could discard the value we
// didn't want with the `_` operator:
// let _, repo_stars = github_stars

printfn $"The GitHub repo '{repo_name}' has {repo_stars} stars"

// It is worth noting that while tuples generally contain two values this is not
// a hard limit and tuples can contain any number of values.

// Tuples are specifically groupings of UNNAMED values, which can cause problems
// if we need to identify what the values represent, we can name our values with
// records.

// We desfine a record with the `type` kayword and using curly brackets instead
// instead of parentheses:
type GithubStars = { Repo: string; Stars: int }

// We create a new record by binding values to the record labels, based on the
// record labels the compiler is able to infer the type of record:
let github_stars_1 = { Repo = "dotnet/fsharp"; Stars = 2800 }

// We access the values in a record with dot notation:
printfn $"The GitHub repo '{github_stars_1.Repo}' has {github_stars_1.Stars} stars"

// In F# all values are immutable by default, so if we want to change a single
// value in a record we copy the original record and update the value using the
// `with` keyword:
let github_stars_2 = { github_stars_1 with Stars = 2900 }
printfn $"The GitHub repo '{github_stars_2.Repo}' has {github_stars_2.Stars} stars"

// To work with members we can use the `member` keyword to define functions
// specific to this type of record after the record definition:
type GithubStarsWithMembers =
    { MRepo: string; MStars: int }
    // when accessing members we use the `this` keyword
    member this.GetRepoUrl () =
        $"https://github.com/{this.MRepo}"
    // otherwise we can use the `_` operator
    member _.GetUrl () =
        "https://github.com/"

let github_stars_3 = { MRepo = "dotnet/fsharp"; MStars = 2800 }
// Note we access member functions with `()` on the end of their names to
// differentiate them from a records fields:
printfn $"{github_stars_3.GetRepoUrl()}"

// Discriminated unions let us define unions of different value types we may
// want to apply to our records, again we define them with the `type` keyword
// and list our types with the `|` operator:
type RepoState =
    | Archived
    | Active of {| Maintainer: string |} // here we add an anonymous record to define a Maintainer string

type GithubStarsWithState =
    { Repo: string; Stars: int; State: RepoState }

let (github_stars_4: GithubStarsWithState) =
    // because our state is `Active` we also need to specify a Maintainer value:
    { Repo = "dotnet/fsharp"; Stars = 3000; State = Active {|Maintainer = "Microsoft"|} }

let (github_stars_5: GithubStarsWithState) =
    // here our `Archived` state doesn't require a Maintaniner value:
    { Repo = "dotnet/fsharp"; Stars = 3000; State = Archived }

(* Classes & Interfaces *)
// Like C#, F# has a notion of classes, with properties and methods, to define a
// class we again use the `type` keyword:
type Repo(name: string, stars: int) =
    // we have private properties and methods, denoted by the `let` keyword
    let base_url = "https://github.com/"

    let increment_stars_by stars num = stars + num

    // we can define a constructor
    new() = Repo("", 0)

    // we have instance properties, denoted by the `member` keyword
    // which are immutable by default
    member this.Name = name
    // or can be made mutable with the `val` keyword
    member val Stars = stars with get, set

    // we have static methods
    // (note method names end with `()`)
    static member PrintHelp() = "Class that contains repo information"

    // and instance methods
    member _.GetBaseUrl() = $"{base_url}"
    member this.GetRepoUrl() = $"{base_url}{this.Name}"
    member this.IncrementStarsBy(n) = this.Stars <- increment_stars_by this.Stars n

// Accessing a static method with no instance of the class
printfn $"{Repo.PrintHelp()}"

// Create an instance of our class
let fsharp_repo = Repo("dotnet/fsharp", 3100)
// or we can create a blank instance
// let blank_repo = Repo()

// And we can access members and properties of our instance using dot notation
printfn $"{fsharp_repo.Name}"
printfn $"{fsharp_repo.GetRepoUrl()}"
fsharp_repo.Stars <- 3200
fsharp_repo.IncrementStarsBy(50)
printfn $"{fsharp_repo.Stars}"

// Interfaces are like classes without an implementation, they define the 'what'
// of a class, but not the 'how', which is left to the specific implemenation.
// For example, who different classes (Cat and Dog) may implement a Noise()
// method, but the individual classes implement this method differently.
// Additionally, F# also has Object Expressions that simplify the process of
// implementing an interface.

// Interfaces are again defined with the `type` keyword, but members are
// abstract and only contain type annonations
type IHtmlParser =
    abstract member ParseHtml : string -> HtmlDocument

// To implement the inteface we use the `interface` and `with` keywords
type WebParser () =
    interface IHtmlParser with
        member this.ParseHtml url = HtmlDocument.Load(url)

    // to expose interface members we add methods
    member this.ParseHtml url = (this :> IHtmlParser).ParseHtml(url)

type FileParser () =
    interface IHtmlParser with
        member this.ParseHtml filepath =
            filepath
            |> File.ReadAllText
            |> fun x -> HtmlDocument.Parse(x)
    member this.ParseHtml filepath = (this :> IHtmlParser).ParseHtml(filepath)

// create parsers
let class_web_parser = WebParser() :> IHtmlParser
let class_file_parser = FileParser() :> IHtmlParser

// function to handle HTML parsing
let parse_html (parser: IHtmlParser) (source: string) = parser.ParseHtml(source)

// use web parser
printfn $"Web parse: {parse_html class_web_parser (fsharp_repo.GetRepoUrl())}"

let path = Path.Join(__SOURCE_DIRECTORY__, "example.html")
let content = parse_html class_web_parser (fsharp_repo.GetRepoUrl())
File.WriteAllText(path, content.ToString())

// use file parser
printfn $"File parser: {parse_html class_file_parser path}"

// Object Expressions implyfy the process of implementing interfaces
// They are enclosed in curby braces `{ ... }`

let web_parser =
    { new IHtmlParser with
        member this.ParseHtml url = HtmlDocument.Load(url) }

let file_parser =
    { new IHtmlParser with
        member this.ParseHtml filepath =
            filepath
            |> File.ReadAllText
            |> fun x -> HtmlDocument.Parse(x) }

printfn $"Web parse 2: {parse_html web_parser (fsharp_repo.GetRepoUrl())}"
printfn $"File parser 2: {parse_html file_parser path}"

(* COLLECTIONS *)
// Collections are types which contain a series of items of the same type

// List - immutable, ordered series of items
let my_list = [1; 2; 3; 4; 5; 6; 7; 8; 9; 10;]
printfn $"{my_list}"
// we can create a list with the in-built creation functions
// (the type of the list is inferred by the compiler here, so `<int>` isn't needed)
let my_list_2 = List.init<int> 10 (fun i -> i + 1)
printfn $"{my_list_2}"
// or we can use inline initialisation, like for `my_list` above
// or with a range
let my_list_3 = [1 .. 10]
printfn $"{my_list_3}"

// Array - mutable, random access, fixed size, multidimensional
let my_array = [|1; 2; 3|]
// like with lists we can use a array creation functions
let my_array_2 = Array.init<int> 10 (id)
// and we can also initialise multi dimensional arrays
let my_array_2d = Array2D.init<int> 3 3 (fun x _ -> x)
let my_array_3d = Array3D.init<int> 3 3 3 (fun _ _ z -> z)
// like lists, arrays can be initialised inline including with a range
let my_array_3 = [|1; 2; 3; 4; 5; 6; 7; 8; 9; 10|]
let my_array_4 = [|1 .. 10|]

// Sequence - immutable, items computed as needed
let my_seq = seq {1; 2; 3}
// once again, sequences can be initialised with creation functions
let my_seq_2 = Seq.init<int> 10 (fun i -> i)
// because sequences are computed we can technicall have an infinite sequence,
// whose items are calculated from the lambda function we initialise it with
let my_seq_3 = Seq.initInfinite (fun i -> i)
// once again, sequences can be initialised inline and with ranges
// but must be initialised with the `seq§ keyword
let my_seq_4 = seq {1; 2; 3; 4; 5; 6; 7; 8; 9; 10}
let my_seq_5 = seq {1 .. 10}

// Map - immutable, key-value pairs

// Set - immutable, sitinct items

(* ACCESSING ELEMENTS *)
type Transaction =
    {   Date: DateTime
        CustomerId: string
        Amount: double  }

let transactions =
    [
        {   Date = new DateTime(2024,8,2)
            CustomerId = "Acme Limited"
            Amount = 2400.00    }
        {   Date = new DateTime(2024,8,3)
            CustomerId = "Loony Tunes Inc"
            Amount = 1500.00    }
        {   Date = new DateTime(2024,8,3)
            CustomerId = "Acme Limited"
            Amount = 1800.00    }
    ]

// returns the first item
printfn $"{transactions.Head}"

// returns the remaining items
printfn $"{transactions.Tail}"

// we can access list items by index, although this is more efficient on arrays
// returns the first item (by index)
printfn $"{transactions.[0]}"

// returns the remaining items (by index range)
printfn $"{transactions.[1..]}"

// we can use built-in collection operations
transactions
    // attemptimng to `.find` a non-exitent customer id would cause an error!
    |> List.find (fun t -> t.CustomerId = "Acme Limited")
    |> printfn "Result: %A"

// Use `.tryFind` where an item may not exist, this will return an option type
// so if no item is found it will return None.
transactions
    |> List.tryFind (fun t -> t.CustomerId = "Acme Ltd")
    |> printfn "Result: %A"

// We can append and prepend a list
let to_dos = ["Learn F#"; "Build a killer F# app"; "$$$"]

// append
let to_dos_updated = ["Repeat"] |> List.append to_dos
// prepend
let to_dos_updated_again = to_dos_updated |> List.append ["Make coffee"]

printfn $"{to_dos_updated_again}"

// we can convert collections
let to_do_array = to_dos |> Array.ofList
let to_do_seq = to_dos |> Seq.ofList

// we can apply various operations to lists
// map
transactions
    |> List.map
        ( fun t ->
            let vat = 0.20
            {|  before_vat = t.Amount
                vat_added = t.Amount * vat
                total = t.Amount * (1.0 + vat)  |})
    |> printfn "%A"

// iter
transactions
    |> List.iter (fun t -> printfn $"{t.CustomerId}")

// sumBy
transactions
    |> List.sumBy (fun t -> t.Amount)
    |> printfn "%A"

// averageBy
transactions
    |> List.averageBy (fun t -> t.Amount)
    |> printfn "%A"

// filter
transactions
    |> List.filter (fun t -> t.Date > DateTime(2024, 8, 2))
    |> printfn "%A"

// sortBy
transactions
    |> List.sortBy (fun t -> t.Amount)
    |> printfn "%A"

// sortByDescending
transactions
    |> List.sortByDescending (fun t -> t.Amount)
    |> printfn "%A"

// these operations can be combined using pipelines
// here we filter for transactions greater than 1500.00 and sort by desc date
transactions
    |> List.filter (fun t -> t.Amount > 1500.00)
    |> List.sortByDescending (fun t -> t.Date)
    |> printfn "%A"