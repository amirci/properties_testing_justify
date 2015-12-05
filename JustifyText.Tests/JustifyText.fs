namespace JustifyText.Tests

open FsCheck
open FsCheck.NUnit
open JustifyText
open System.Text.RegularExpressions

module ``The justified text`` = 
    let replaceMultipleSpaces str = str 
    [<Property(Arbitrary=[|typeof<Generators>|])>]
    let ``Has the same words as the unjustified`` (text: RandomParagraph) =
        text
        |> justify
        |> all
        |> replaceMultipleSpaces
        |> concatWords
        |> (=) text.Get

module ``Each line of the justified body`` =
    let ``starts and ends with a word`` line = Regex.Match(line, "^\w.*\w$").Success
    
    [<Property(Arbitrary=[|typeof<Generators>|])>]
    let ``Starts and ends with a word`` (text: RandomParagraph) =
        text
        |> justify
        |> body
        |> Seq.forall ``starts and ends with a word``

module ``The last line`` =
    let ``spaces are one less than words`` (words, spaces) = spaces = words - 1

    [<Property(Arbitrary=[|typeof<Generators>|])>]
    let ``Has only one space in between words`` (text: RandomParagraph) =
        text
        |> justify
        |> lastLine
        |> countWordsAndSpaces
        |> ``spaces are one less than words``

