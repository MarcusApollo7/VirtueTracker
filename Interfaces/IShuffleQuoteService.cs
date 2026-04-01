using VirtueTracker.Models;

namespace VirtueTracker.Interfaces;
public interface IShuffleQuoteService
{
    public Task<Quote> GetNextQuoteAsync(string virtueId);
}