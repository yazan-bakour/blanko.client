using System.Text;
using System.Text.Json;
using Banko.Client.Models;

namespace Banko.Client.Services
{
  public class ErrorService(JsonSerializerOptions jsonSerializerOptions)
  {
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

    /// <summary>
    /// Processes an unsuccessful HttpResponseMessage, extracts error details,
    /// and throws an HttpRequestException.
    /// </summary>
    /// <param name="response">The HttpResponseMessage to handle.</param>
    public async Task HandleHttpResponseErrorAsync(HttpResponseMessage response)
    {
      string rawErrorContent = await response.Content.ReadAsStringAsync();
      var errorMessages = new StringBuilder();
      try
      {
        var problemDetails = JsonSerializer.Deserialize<ValidationErrors>(rawErrorContent, _jsonSerializerOptions);
        if (problemDetails != null)
        {
          if (!string.IsNullOrWhiteSpace(problemDetails?.Message))
          {
            errorMessages.AppendLine(problemDetails.Message);
          }
          if (problemDetails.HasErrors())
          {
            // Try dictionary format first (validation problem details)
            var errorsDictionary = problemDetails.GetErrorsAsDictionary();
            if (errorsDictionary != null)
            {
              foreach (var error in errorsDictionary)
              {
                foreach (var message in error.Value)
                {
                  errorMessages.AppendLine($"    {message}");
                }
              }
            }
          }
          else
          {
            // Try array format
            var errorsArray = problemDetails.GetErrorsAsArray();
            if (errorsArray != null)
            {
              foreach (var error in errorsArray)
              {
                // Don't duplicate the message if it's already added
                if (!errorMessages.ToString().Contains(error))
                {
                  errorMessages.AppendLine(error);
                }
              }
            }
          }
        }
      }
      catch (JsonException)
      {
        errorMessages.AppendLine(rawErrorContent);
      }
      throw new HttpRequestException(errorMessages.ToString(), null, response.StatusCode);
    }
  }
}