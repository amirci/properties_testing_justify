namespace JustifyText.Tests

open FsCheck
open FsCheck.NUnit
open JustifyText
open System.Text.RegularExpressions
open Util

module ``The justified text`` = 
    let replaceMultipleSpaces str = str 
    [<JustifyProperty>]
    let ``Has the same words as the unjustified`` (text: RandomParagraph) =
        text
        |> justify
        |> all
        |> concatLines
        |> replaceMultipleSpaces
        |> (=) text.Get

module ``Each line of the justified body`` =
    let ``starts and ends with a word`` line = Regex.Match(line, "^[^ ].*[^ ]$").Success
    
    [<JustifyProperty>]
    let ``Starts and ends with a word`` (text: RandomParagraph) =
        text
        |> justify
        |> body
        |> Seq.forall ``starts and ends with a word``

module ``The last line`` =
    let ``spaces are one less than words`` (words, spaces) = spaces = words - 1
    let isWordChar = (<>) ' '
    let countChars = Seq.filter isWordChar >> Seq.length

    [<JustifyProperty>]
    let ``Is shorter than the max width`` (text: RandomParagraph) =
        text 
        |> justify
        |> lastLine
        |> countChars
        |> (>=) length

    [<JustifyProperty>]
    let ``Has only one space in between words`` (text: RandomParagraph) =
        text
        |> justify
        |> lastLine
        |> countWordsAndSpaces
        |> ``spaces are one less than words``

