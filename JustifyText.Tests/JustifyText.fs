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

//    [<JustifyProperty>]
//    let ``Has the maximum amount of words`` (text: RandomParagraph) =
//        // if a line i has n words
//        // adding the first word of line i+1 
//        // plus adding one space between words
//        // is greather than the length
//        let toPairs lines = lines |> Seq.skip 1 |> Seq.zip lines
//        let isMax (line, next) = false
//
//        text 
//        |> justify 
//        |> all
//        |> toPairs
//        |> Seq.forall isMax


module ``Each line of the justified body`` =
    [<JustifyProperty>]
    let ``Has the max width`` (text: RandomParagraph) =
        text
        |> justify
        |> body
        |> Seq.forall (String.length >> (=) length)
        

module ``The last line`` =
    let ``spaces are one less than words`` (words, spaces) = spaces = words - 1

    [<JustifyProperty>]
    let ``Has only one space in between words`` (text: RandomParagraph) =
        text
        |> justify
        |> lastLine
        |> countWordsAndSpaces
        |> ``spaces are one less than words``

    [<JustifyProperty>]
    let ``Fits the max width`` (text: RandomParagraph) =
        text 
        |> justify
        |> all
        |> Seq.forall (countChars >> (>=) length)
