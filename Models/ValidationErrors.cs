using System.Text.Json.Serialization;
using System.Text.Json;

namespace Banko.Client.Models
{
  public class ValidationErrors
  {
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("errors")]
    public JsonElement Errors { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    public Dictionary<string, string[]>? GetErrorsAsDictionary()
    {
      if (Errors.ValueKind == JsonValueKind.Object)
      {
        return Errors.Deserialize<Dictionary<string, string[]>>();
      }
      return null;
    }
    public string[]? GetErrorsAsArray()
    {
      if (Errors.ValueKind == JsonValueKind.Array)
      {
        return Errors.Deserialize<string[]>();
      }
      return null;
    }
    public bool HasErrors()
    {
      return Errors.ValueKind != JsonValueKind.Undefined &&
             Errors.ValueKind != JsonValueKind.Null;
    }
  }
}