using MauiBlazorWeb.Shared.Models;
using MauiBlazorWeb.Shared.Interfaces;

namespace MauiBlazorWeb.Services;

public class VirtueRepository : IVirtueRepository
{
    private readonly IDatabaseService _db;

    public VirtueRepository(IDatabaseService db)
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
