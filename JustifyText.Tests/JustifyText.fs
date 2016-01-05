namespace JustifyText.Tests

open FsCheck
open FsCheck.NUnit
open JustifyText
open System.Text.RegularExpressions
open Util
open Swensen.Unquote

module ``The justified text`` = 
    [<JustifyProperty>]
    let ``Has the same words as the unjustified`` (text: RandomParagraph) =
        let replaceMultipleSpaces str = Regex("\\s+").Replace(str, " ")
        let actual = text |> justify |> all |> concatLines |> replaceMultipleSpaces 
        actual =! text.Get
   
module ``Every line of the justified text`` = 

    [<JustifyProperty>]
    let ``Starts and ends with a word`` (text: RandomParagraph) =
        let ``starts and ends with a word`` line = Regex.Match(line, "^[^ ].*[^ ]$").Success
        text
        |> justify
        |> all
        |> Seq.forall ``starts and ends with a word``


module ``Each line of the justified body`` =
    [<JustifyProperty>]
    let ``Has the max width`` (text: RandomParagraph) =
        let lines = text |> justify |> body 
        let lengths = lines |> Seq.map String.length |> List.ofSeq
        lengths =! List.init (lines |> Seq.length) (fun _ -> length)
       
    [<JustifyProperty>]
    let ``Large gaps go first then from smaller`` (text:RandomParagraph) =
        let spaces = Regex("\\s+").Matches(text.Get)
        let gaps = [for space in spaces -> space.Length]
        gaps = (gaps |> List.sort |> List.rev)
    
    [<JustifyProperty>]
    let ``The space gaps have at most difference of one`` (text:RandomParagraph) =
        let lines = text |> justify |> body
        
        let actual = 
            let findGaps line = [for g in Regex("\\s+").Matches(line) -> g.Value]
            lines |> Seq.map (findGaps >> List.map String.length >> Seq.distinct >> List.ofSeq) |> List.ofSeq
        
        let expected = 
            let expectedGaps = function | a::b::[] -> a::(a-1)::[] | l -> l
            actual |> List.map expectedGaps

        expected =! actual

    [<JustifyProperty>]
    let ``Can not fit another word`` (text: RandomParagraph) =
        let toPairs lines = lines |> Seq.skip 1 |> Seq.zip lines
        let wontFitAnother (line, next) = 
            let firstWord = (next |> toWords).[0]  |> String.length |> (+) 1
            line |> toWords |> totalWithSpaces |> (+) firstWord > length

        text 
        |> justify 
        |> all
        |> toPairs
        |> Seq.forall wontFitAnother

module ``The last line`` =

    [<JustifyProperty>]
    let ``Has only one space in between words`` (text: RandomParagraph) =
        let ``has only one space between words`` (line:string) = line.Split(' ') |> Seq.forall ((<>) "")
            
        text
        |> justify
        |> lastLine
        |> ``has only one space between words``

    [<JustifyProperty>]
    let ``Fits the max width`` (text: RandomParagraph) =
        text 
        |> justify
        |> all
        |> Seq.forall (countChars >> (>=) length)
