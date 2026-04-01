namespace VirtueTracker.Services.Diagnostics;

public class DatabaseHealthReport
{
    public bool FileExists { get; set; }
    public long FileSizeBytes { get; set; }

    public int VirtueCount { get; set; }
    public int MeaningCount { get; set; }
    public int MeaningTextCount { get; set; }
    public int QuoteCount { get; set; }
    public int QuoteTextCount { get; set; }

    public List<string> Errors { get; set; } = new();
    public bool IsHealthy => Errors.Count == 0;
}