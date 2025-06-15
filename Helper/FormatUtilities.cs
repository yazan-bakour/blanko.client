using System.Security.Cryptography;
using MudBlazor;
using Banko.Client.Models.Transaction;

namespace Banko.Client.Helper
{
  public static class FormatUtilities
  {
    public static string GetCurrencySymbol(string isoCode)
    {
      var region = System.Globalization.CultureInfo
        .GetCultures(System.Globalization.CultureTypes.SpecificCultures)
        .Select(c => new System.Globalization.RegionInfo(c.Name))
        .FirstOrDefault(r => r.ISOCurrencySymbol == isoCode.ToUpper());
      return region?.CurrencySymbol ?? isoCode;
    }
    public static string GetFileHash(byte[] fileData)
    {
      using var sha256 = SHA256.Create();
      var hash = sha256.ComputeHash(fileData);
      return Convert.ToHexString(hash).Substring(0, 16); // short 16-char hash
    }
    public static string FormatBytes(long bytes)
    {
      string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
      int i = 0;
      double dblSByte = bytes;

      if (bytes > 0)
      {
        for (i = 0; (bytes / 1024) > 0 && i < suffixes.Length - 1; i++, bytes /= 1024)
        {
          dblSByte = bytes / 1024.0;
        }
      }
      else
      {
        dblSByte = 0;
      }

      return $"{dblSByte:0.##} {suffixes[i]}";
    }

    public static string GetTransactionInitials(string? description)
    {
      if (string.IsNullOrWhiteSpace(description)) return "T";

      var parts = description.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
      if (parts.Length == 0) return description.Substring(0, Math.Min(description.Length, 1)).ToUpper();

      if (parts.Length == 1) return parts[0].Substring(0, Math.Min(parts[0].Length, 2)).ToUpper();

      return $"{parts[0][0]}{parts[1][0]}".ToUpper();
    }

    public static string GetAmountInitials(string currentAccountNumbers, string? sourceAccountNumber, string? destinationAccountNumber)
    {
      try
      {
        // Split the comma-separated string into distinct account numbers
        var accountList = currentAccountNumbers
            .Split([','], StringSplitOptions.RemoveEmptyEntries)
            .Select(a => a.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var source = sourceAccountNumber ?? string.Empty;
        var destination = destinationAccountNumber ?? string.Empty;

        bool isSourceMine = accountList.Contains(source);
        bool isDestinationMine = accountList.Contains(destination);

        if (isSourceMine && isDestinationMine) return "+";
        if (isSourceMine && !isDestinationMine) return "-";
        if (!isSourceMine && isDestinationMine) return "+";

        // Default if none of the above conditions match
        return "?";
      }
      catch
      {
        // Fallback for any unexpected errors
        return "?";
      }
    }

    public static Color GetStatusColor(TransactionStatus status) => status switch
    {
      TransactionStatus.Completed => Color.Success,
      TransactionStatus.Pending => Color.Info,
      TransactionStatus.Created => Color.Info,
      TransactionStatus.Failed => Color.Error,
      TransactionStatus.Cancelled => Color.Warning,
      TransactionStatus.Refunded => Color.Primary,
      TransactionStatus.Disputed => Color.Secondary,
      _ => Color.Dark
    };
  }
}
