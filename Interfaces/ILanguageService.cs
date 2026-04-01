namespace VirtueTracker.Interfaces;
public interface ILanguageService
{
    string CurrentLanguage { get; }
    event Action? OnLanguageChanged;

    void SetLanguage(string languageCode);
}