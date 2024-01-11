module Extensions

open System.Runtime.CompilerServices
open Flurl.Http
open System.Text.Json
open System.Text.Json.Serialization

/// <summary>
/// A type that provides extension methods for Flurl requests
/// </summary>
[<Extension>]
type FlurlExtensions() =
    static let serializerOptions: JsonSerializerOptions =
        let options = JsonSerializerOptions(PropertyNameCaseInsensitive = true)
        options.Converters.Add(JsonStringEnumConverter())
        options

    /// <summary>
    /// Gets the JSON response from a Flurl request
    /// </summary>
    /// <param name="req">The Flurl request</param>
    /// <returns>The deserialized JSON object of type 'T</returns>
    [<Extension>]
    static member GetJson<'T>(req: IFlurlRequest) =
        let str = req.GetStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
        JsonSerializer.Deserialize<'T>(str, serializerOptions)

