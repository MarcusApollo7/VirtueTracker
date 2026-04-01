using SQLite;
using VirtueTracker.Interfaces;
using VirtueTracker.Models;

namespace VirtueTracker.Services;

public class QuoteRepository : IQuoteRepository
{
    private readonly SQLiteAsyncConnection _db;

    public QuoteRepository(DatabaseService databaseService)
    {
        _db = databaseService.Connection;
    }

    public Task<List<Quote>> GetAllQuotesAsync()
        => _db.Table<Quote>().ToListAsync();
public Task<List<Quote>> GetAllQuotesVirtueAsync(string virtueId)
    => _db.Table<Quote>().Where(q => q.VirtueId == virtueId).ToListAsync();


    public Task<Quote> GetQuoteAsync(string id)
        => _db.Table<Quote>().Where(q => q.Id == id).FirstOrDefaultAsync();

    public Task<int> AddQuoteAsync(Quote quote)
        => _db.InsertAsync(quote);

    public Task<int> AddQuoteTextAsync(QuoteText text)
        => _db.InsertAsync(text);

    public Task<List<QuoteText>> GetQuoteTextsAsync(string quoteId)
        => _db.Table<QuoteText>().Where(t => t.QuoteId == quoteId).ToListAsync();
}
