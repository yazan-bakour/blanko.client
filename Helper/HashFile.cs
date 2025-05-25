using System.Security.Cryptography;

namespace Banko.Client.Helper
{
  public static class HashFile
  {
    public static string GetFileHash(byte[] fileData)
    {
      using var sha256 = SHA256.Create();
      var hash = sha256.ComputeHash(fileData);
      return Convert.ToHexString(hash).Substring(0, 16); // short 16-char hash
    }
  }
}
