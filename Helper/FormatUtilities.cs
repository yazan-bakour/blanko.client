using System.Security.Cryptography;

namespace Banko.Client.Helper
{
  public static class FormatUtilities
  {
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
  }
}
