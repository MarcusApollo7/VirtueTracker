using VirtueTracker.Models;

namespace VirtueTracker.Interfaces;

public interface IMeaningRepository
{
    Task<Meaning> GetMeaningAsync(string virtueId);
    Task<int> AddMeaningAsync(Meaning meaning);
    Task<List<MeaningText>> GetMeaningTextsAsync(string meaningId);
}
