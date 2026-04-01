using VirtueTracker.Interfaces;
using VirtueTracker.Models;

namespace VirtueTracker.Services;

public class ShuffleQuoteService : IShuffleQuoteService
{
    private readonly QuoteRepository _repo;
    private Dictionary<string, List<Quote>> _quotes = [];
    private readonly Random _random = new();
    public ShuffleQuoteService(QuoteRepository repo)
    {
        _repo = repo;
    }
    public async Task<Quote> GetNextQuoteAsync(string virtueId)
    {
        if (!_quotes.ContainsKey(virtueId) || _quotes[virtueId].Count == 0)
        {
            List<Quote> quotes = await _repo.GetAllQuotesVirtueAsync(virtueId);
            _quotes[virtueId] = [.. quotes.OrderBy(q => _random.Next())];
        }
        Console.WriteLine($"Quotes for {virtueId}: {_quotes[virtueId].Count}");
        Quote next = _quotes[virtueId][0];
        _quotes[virtueId].RemoveAt(0);
        
        return next;
    }
}