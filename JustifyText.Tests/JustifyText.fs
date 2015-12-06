namespace JustifyText.Tests

open FsCheck
open FsCheck.NUnit
open JustifyText
open System.Text.RegularExpressions
open Util

module ``The justified text`` = 
    let replaceMultipleSpaces str = Regex("\\s+").Replace(str, " ")
    [<JustifyProperty>]
    let ``Has the same words as the unjustified`` (text: RandomParagraph) =
        text
        |> justify
        |> all
        |> concatLines
        |> replaceMultipleSpaces
        |> (=) text.Get
   
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
        text
        |> justify
        |> body
        |> Seq.forall (String.length >> (=) length)
        
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
