using System.Globalization;

namespace Banko.Client.Helper;

public static class CurrencyFormatter
{
  public static string FormatCurrency(decimal amount)
  {
    return amount.ToString("C2", CultureInfo.CurrentCulture);
  }
}