namespace VirtueTracker.SeedModels;

public class VirtueSeed
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public List<MeaningSeed> Meanings { get; set; } = default!;
    public List<QuoteSeed> Quotes { get; set; } = default!;
}

public class MeaningSeed
{
    public string Id { get; set; } = default!;
    public List<MeaningTextSeed> Texts { get; set; } = default!;
}

public class MeaningTextSeed
{
    public string Id { get; set; } = default!;
    public string Language { get; set; } = default!;
    public string Text { get; set; } = default!;
}

public class QuoteSeed
{
    public string Id { get; set; } = default!;
    public string Author { get; set; } = default!;
    public List<QuoteTextSeed> Texts { get; set; } = default!;
}

public class QuoteTextSeed
{
    public string Id { get; set; } = default!;
    public string Language { get; set; } = default!;
    public string Text { get; set; } = default!;
}

public class RootSeed
{
    public List<VirtueSeed> Virtues { get; set; } = default!;
}