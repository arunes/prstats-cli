module Tests

open System
open Xunit
open System.IO


[<Fact>]
let ``printCommandHeader should print the version and command name`` () =
    use sw = new StringWriter()
    Console.SetOut sw
    
    let command = "test"
    let expected = sprintf "prstats-cli v0.1 - running '%s' command.\r\n\r\n" command
    Utils.printCommandHeader command
    let actual = sw.ToString()
    Assert.Equal(expected, actual)

