
using VirtueTracker.Models;

namespace VirtueTracker.Services;
public class LanguageService
{
    private string _currentLanguage = "en";

    public string CurrentLanguage => _currentLanguage;

    public event Action? OnLanguageChanged;

    public void SetLanguage(string languageCode)
    {
        if (_currentLanguage == languageCode)
            return;

        _currentLanguage = languageCode;
        OnLanguageChanged?.Invoke();
    }
}