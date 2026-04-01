using SQLite;

namespace VirtueTracker.Models;

public class Quote
{
    public string VirtueId {get; set;} = string.Empty;
    [PrimaryKey]
    public string Id {get; set;} = Guid.NewGuid().ToString();
    public string Author {get; set;} = string.Empty;
    public string Source {get; set;} = string.Empty;
    public string OLangText {get; set;} = string.Empty;
    public DateTime? Date {get; set;}
}

public class QuoteText
{
    [PrimaryKey]
    public string Id {get; set;} = Guid.NewGuid().ToString();
    public string QuoteId {get; set;} = string.Empty;
    public string LanguageCode {get; set;} = string.Empty;
    public string Text {get; set; } = string.Empty;
}