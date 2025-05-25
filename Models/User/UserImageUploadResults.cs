namespace Banko.Client.Models.User
{
  public enum ImageSource
  {
    Url,
    File
  }

  public class UserImageUploadResults
  {
    public ImageSource ImageSource { get; set; }
    public string ImageData { get; set; } = string.Empty;
  }
}