using SQLite;
using VirtueTracker.Models;

namespace VirtueTracker.Services;
public class MeaningRepository
{
    private readonly SQLiteAsyncConnection _db;

    public MeaningRepository(DatabaseService databaseService)
    {
        _db = databaseService.Connection;
    }
    public Task<Meaning> GetMeaningAsync(string virtueId)
    => _db.Table<Meaning>().Where(m => m.VirtueId == virtueId).FirstOrDefaultAsync();
    public Task<int> AddMeaningAsync(Meaning meaning)
        => _db.InsertAsync(meaning);

    public Task<int> AddMeaningTextAsync(Meaning meaning)
        => _db.InsertAsync(meaning);

    public Task<List<MeaningText>> GetMeaningTextsAsync(string meaningId)
        => _db.Table<MeaningText>().Where(t => t.MeaningId == meaningId).ToListAsync();
}
