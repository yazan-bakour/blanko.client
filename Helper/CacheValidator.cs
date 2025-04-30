namespace Banko.Client.Helper;

public interface ICacheValidator<T>
{
  bool IsInitialized { get; }
  T? Data { get; }
  void UpdateCache(bool isInitialized, T? data);
  event Action? OnStateChanged;
}

public class CacheValidator<T> : ICacheValidator<T>
{
  private bool _isInitialized;
  private T? _data;

  public bool IsInitialized => _isInitialized;
  public T? Data => _data;
  public event Action? OnStateChanged;

  public void UpdateCache(bool isInitialized, T? data)
  {
    _isInitialized = isInitialized;
    _data = data;
    NotifyStateChanged();
  }
  // Notifier can be its own helper
  private void NotifyStateChanged()
  {
    OnStateChanged?.Invoke();
  }
}