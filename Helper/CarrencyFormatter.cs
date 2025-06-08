using System.Globalization;

namespace Banko.Client.Helper;

public static class CurrencyFormatter
{
  public static string FormatCurrency(decimal amount)
  {
    return amount.ToString("C2", CultureInfo.CurrentCulture);
  }
  public static string FormatCurrencySymbol(decimal amount, string currency)
  {
    try
    {
      // Create a NumberFormatInfo based on the currency
      var cultureInfo = GetCultureInfoForCurrency(currency);
      var numberFormat = cultureInfo.NumberFormat.Clone() as NumberFormatInfo;

      if (numberFormat != null)
      {
        // Set the currency symbol based on the currency code
        numberFormat.CurrencySymbol = GetCurrencySymbol(currency);

        // Ensure symbol is placed before the number for consistency
        numberFormat.CurrencyPositivePattern = 0; // $n (symbol before, no space)
        numberFormat.CurrencyNegativePattern = 0; // ($n) or -$n depending on culture

        return amount.ToString("C2", numberFormat);
      }
    }
    catch
    {
      // Fallback to default formatting if currency lookup fails
    }

    // Fallback: use default format with symbol before number
    var symbol = GetCurrencySymbol(currency);
    return amount >= 0 ? $"{symbol}{amount:N2}" : $"-{symbol}{Math.Abs(amount):N2}";
  }
  private static CultureInfo GetCultureInfoForCurrency(string currency)
  {
    return currency.ToUpper() switch
    {
      "USD" => new CultureInfo("en-US"),
      "EUR" => new CultureInfo("de-DE"),
      "GBP" => new CultureInfo("en-GB"),
      "JPY" => new CultureInfo("ja-JP"),
      "CAD" => new CultureInfo("en-CA"),
      "AUD" => new CultureInfo("en-AU"),
      "CHF" => new CultureInfo("de-CH"),
      "CNY" => new CultureInfo("zh-CN"),
      "SEK" => new CultureInfo("sv-SE"),
      "NOK" => new CultureInfo("nb-NO"),
      "DKK" => new CultureInfo("da-DK"),
      "PLN" => new CultureInfo("pl-PL"),
      "CZK" => new CultureInfo("cs-CZ"),
      "HUF" => new CultureInfo("hu-HU"),
      "RUB" => new CultureInfo("ru-RU"),
      "BRL" => new CultureInfo("pt-BR"),
      "MXN" => new CultureInfo("es-MX"),
      "INR" => new CultureInfo("hi-IN"),
      "KRW" => new CultureInfo("ko-KR"),
      "SGD" => new CultureInfo("en-SG"),
      "NZD" => new CultureInfo("en-NZ"),
      "ZAR" => new CultureInfo("en-ZA"),
      _ => CultureInfo.CurrentCulture
    };
  }
  private static string GetCurrencySymbol(string currency)
  {
    return currency.ToUpper() switch
    {
      "USD" => "$",
      "EUR" => "€",
      "GBP" => "£",
      "JPY" => "¥",
      "CAD" => "C$",
      "AUD" => "A$",
      "CHF" => "CHF",
      "CNY" => "¥",
      "SEK" => "kr",
      "NOK" => "kr",
      "DKK" => "kr",
      "PLN" => "zł",
      "CZK" => "Kč",
      "HUF" => "Ft",
      "RUB" => "₽",
      "BRL" => "R$",
      "MXN" => "$",
      "INR" => "₹",
      "KRW" => "₩",
      "SGD" => "S$",
      "NZD" => "NZ$",
      "ZAR" => "R",
      _ => currency.ToUpper() // Fallback to currency code
    };
  }
}