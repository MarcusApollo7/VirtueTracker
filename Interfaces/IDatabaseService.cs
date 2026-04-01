using SQLite;
using VirtueTracker.Services.Diagnostics;

namespace VirtueTracker.Interfaces;
public interface IDatabaseService
{
    public bool IsReady {get; }
    Task InitializeAsync();
    Task ResetDatabaseAsync();
    public SQLiteAsyncConnection Connection {get; }
    Task<DatabaseHealthReport> CheckHealthAsync();

}