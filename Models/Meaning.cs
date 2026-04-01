using SQLite;

namespace VirtueTracker.Models;

public class Meaning
{
    [PrimaryKey]
    public string Id {get; set;} = Guid.NewGuid().ToString();
    public string VirtueId {get; set;} = string.Empty;
    public string Text {get; set;} = string.Empty;

}

public class MeaningText()
{
    // Id is the primary key and MeaningId is the foreign key
    [PrimaryKey]
    public string Id {get; set;} = Guid.NewGuid().ToString();
    public string MeaningId {get; set;} = string.Empty;
    public string LanguageCode {get; set;} = string.Empty;
    public string Text {get; set; } = string.Empty;
}