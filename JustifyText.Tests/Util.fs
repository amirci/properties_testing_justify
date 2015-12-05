namespace JustifyText.Tests

open FsCheck
open FsCheck.NUnit
open JustifyText
open System.Text.RegularExpressions

[<AutoOpen>]
module LoremIpsum = 
    let paragraphs = """
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis lacinia justo vitae enim sagittis hendrerit. Aliquam euismod blandit ante, in mattis nisi fermentum eget. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin in arcu vitae tortor semper ultricies. In vel nunc non nisi egestas bibendum. Nam tempor ut arcu a auctor. Pellentesque pellentesque interdum nisl, et egestas massa pellentesque id. Nullam blandit, leo vel suscipit egestas, libero libero malesuada nibh, iaculis congue nisl nisl non justo. Sed quis massa a justo lacinia maximus eget ut tellus.
        Proin malesuada cursus nulla, vel commodo enim sollicitudin in. Vestibulum gravida nibh eu libero ornare, sit amet facilisis arcu fermentum. Fusce ultricies suscipit facilisis. Nulla ac nisl ac erat euismod porttitor. Mauris blandit lobortis dictum. Vestibulum suscipit neque lacus, eget commodo lectus dapibus at. Donec convallis, sem eu bibendum lacinia, velit quam eleifend orci, ut interdum ante nibh id purus. Maecenas elit nunc, sollicitudin sed tempus vitae, ultrices id erat. Fusce sit amet mi est. Sed iaculis lacus urna, eu rutrum est tristique quis. Donec ac magna vel elit suscipit venenatis. Vestibulum molestie mauris at nunc varius semper. Nulla rhoncus lectus at nunc luctus varius. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla sit amet varius quam, id pulvinar sapien. Etiam dictum, lorem ut elementum ultrices, tellus sem mattis mauris, vel cursus velit turpis vitae nunc. Duis cursus ultrices arcu, eu fermentum elit sodales mattis. Phasellus eu maximus risus. Mauris in vehicula nunc. Morbi at enim porttitor, maximus purus a, ullamcorper augue. Aenean scelerisque turpis vel egestas gravida. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Sed leo risus, scelerisque in diam ut, suscipit vehicula sapien.
        Quisque sagittis iaculis tristique. Duis placerat tellus in magna sollicitudin commodo. Pellentesque at vehicula mauris. Aliquam at finibus est. In dignissim diam eu turpis vehicula dictum vel at risus. Vivamus efficitur sed metus eget molestie. Suspendisse lobortis magna urna, at tincidunt arcu molestie at. Aliquam luctus et lorem eget consectetur. Pellentesque quis feugiat ante. Nullam vehicula libero urna, et placerat orci feugiat quis. Morbi pharetra risus ultrices dolor egestas varius. Nulla eu velit quam. Donec tristique luctus nisi ac varius.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc ac lacus sed diam luctus gravida. Vivamus eget metus odio. Pellentesque consectetur arcu id molestie posuere. Donec id congue velit. Sed ac libero mi. Aliquam erat volutpat. Fusce tempor urna erat, vitae placerat elit blandit at. Maecenas a dui nisl. Donec ac enim orci. Fusce orci metus, vulputate scelerisque risus quis, commodo malesuada leo. Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        """

    let words = 
//        let splitChars = [|'\n';','; '.'; ' '; ';'|]
        let splitChars = [|'\n';'\r'; ' '|]
        paragraphs.Split(splitChars, System.StringSplitOptions.RemoveEmptyEntries) 

[<AutoOpen>]
module Util = 
    type RandomParagraph = RandomParagraph of string with
        member x.Get = match x with RandomParagraph str -> str
        static member op_Explicit(RandomParagraph str) = str

    type Generators = 
        static member Paragraph() =
            Gen.elements words
            |> Gen.listOfLength 20
            |> Gen.map (String.concat " " >> RandomParagraph)
            |> Arb.fromGen
            
    let splitLines (str: string) = str.Split('\n') 
    let toWords (str:string) = str.Split([|' '|], System.StringSplitOptions.RemoveEmptyEntries)
    
    let countWordsAndSpaces (line:string) =
        let wordCount  = line |> toWords |> Seq.length
        let spaceCount = line |> Seq.filter ((=) ' ') |> Seq.length
        wordCount, spaceCount
    
    let isWordChar = (<>) ' '
    let countChars (str:string) = str |> Seq.filter isWordChar |> Seq.length
    let countWithSingleSpace words = (words |> Array.sumBy String.length) + words.Length - 1
    
    let length = 30
    let justify (text:RandomParagraph) = 
        let lines = Justified.Justify(length, text.Get) |> splitLines
        match lines.Length with
        | 1   -> Seq.empty, lines.[0]
        | len -> lines |> Seq.take (max 0 len - 1), lines.[len-1]

    let lastLine = snd
    let body = fst
    let all (lines:seq<string>, line:string) = [line] |> Seq.append lines
    let concatWords words = String.concat " " words
    let concatLines = concatWords

    type JustifyPropertyAttribute() =
        inherit PropertyAttribute( Arbitrary = [| typeof<Generators> |], QuietOnSuccess = true)