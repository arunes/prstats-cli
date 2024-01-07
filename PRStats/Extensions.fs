module Extensions

open System.Runtime.CompilerServices
open Flurl.Http
open System.Text.Json
open System.Text.Json.Serialization

[<Extension>]
type FlurlExtensions() =
    static let serializerOptions: JsonSerializerOptions =
        let options = JsonSerializerOptions(PropertyNameCaseInsensitive = true)
        options.Converters.Add(JsonStringEnumConverter())
        options

    [<Extension>]
    static member GetJson<'T>(req: IFlurlRequest) =
        let str = req.GetStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
        JsonSerializer.Deserialize<'T>(str, serializerOptions)
