using VirtueTracker.Models;

namespace VirtueTracker.Interfaces;

public interface IQuoteRepository
{
    Task<List<Quote>> GetAllQuotesAsync();
    Task<List<Quote>> GetAllQuotesVirtueAsync(string virtueId);
    Task<Quote> GetQuoteAsync(string id);
    Task<int> AddQuoteAsync(Quote quote);
    Task<int> AddQuoteTextAsync(QuoteText text);
    Task<List<QuoteText>> GetQuoteTextsAsync(string quoteId);
}
