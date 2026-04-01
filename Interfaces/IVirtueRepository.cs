using VirtueTracker.Models;

namespace VirtueTracker.Interfaces;

public interface IVirtueRepository
{
    Task<Virtue?> GetVirtueAsync(string virtueId);
}