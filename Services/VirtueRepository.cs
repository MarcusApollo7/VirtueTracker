using VirtueTracker.Models;

namespace VirtueTracker.Services;

public class VirtueRepository
{
    private readonly DatabaseService _db;

    public VirtueRepository(DatabaseService db)
    {
        _db = db;
    }

    public async Task<Virtue?> GetVirtueAsync(string virtueId)
    {
        return await _db.Connection.Table<Virtue>()
            .Where(v => v.Id == virtueId)
            .FirstOrDefaultAsync();
    }
}
