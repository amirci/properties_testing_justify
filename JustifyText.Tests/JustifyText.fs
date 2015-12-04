namespace JustifyText.Tests

open FsCheck
open FsCheck.Xunit

module ``The last line`` =
    [<Property>]
    let ``Has only one space in between words`` (a:int) =
        a > 0

