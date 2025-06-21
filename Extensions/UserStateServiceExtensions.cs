using Banko.Client.Models.User;

namespace Banko.Client.Extensions
{
  static class UserStateServiceExtensions
  {
    public static UserUpdate ToUserUpdate(this UserRead src)
    {
      if (src.User == null) throw new InvalidOperationException("Cannot map UserRead when `User` is null.");
      return new UserUpdate
      {
        Id = src.User.Id,
        Email = src.User.Email,
        FullName = src.User.FullName,
        UniqueId = src.User.UniqueId,
        CreatedAt = src.User.CreatedAt,
        Role = src.User.Role,
        FirstName = src.User.FirstName,
        LastName = src.User.LastName,
        PhoneNumber = src.User.PhoneNumber,
        Address = src.User.Address,
        City = src.User.City,
        State = src.User.State,
        ZipCode = src.User.ZipCode,
        Country = src.User.Country,
        DateOfBirth = src.User.DateOfBirth,
        Nationality = src.User.Nationality,
        Gender = src.User.Gender,
        ProfilePictureDisplay = src.User.ProfilePictureDisplay,
        ProfilePictureUrl = src.User.ProfilePictureUrl,
        Preferences = src.User.Preferences ?? new()
      };
    }
  }
}