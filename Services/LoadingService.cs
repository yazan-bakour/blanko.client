namespace Banko.Client.Services
{
  public class LoadingService
  {
    private bool _isLoading;
    public event Action? OnChange;

    public bool IsLoading
    {
      get => _isLoading;
      set
      {
        if (_isLoading != value)
        {
          _isLoading = value;
          OnChange?.Invoke();
        }
      }
    }
  }
}