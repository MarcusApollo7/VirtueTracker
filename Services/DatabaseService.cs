using SQLite;
using VirtueTracker.Models;
using VirtueTracker.Interfaces;
using VirtueTracker.SeedModels;
using System.Text.Json;
using VirtueTracker.Services.Diagnostics;

namespace VirtueTracker.Services;
public class DatabaseService : IDatabaseService
    {
    private const string DbFileName = "Virtues.db";
    private readonly string _dbPath;
    public SQLiteAsyncConnection Connection { get; private set; } = default!;
    public bool IsReady { get; private set; }

    public DatabaseService()
    {
        Console.WriteLine("DATABASE SERVICE CREATED");
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, DbFileName);
        Console.WriteLine("AppDataDirectory = " + FileSystem.AppDataDirectory);
        Console.WriteLine("DB PATH = " + _dbPath);

    }
    static DatabaseService()
    {
        SQLitePCL.Batteries_V2.Init();
    }


    public async Task InitializeAsync()
    {
        Console.WriteLine("=== INITIALIZING DATABASE ===");

        // Ensure directory exists
        Directory.CreateDirectory(FileSystem.AppDataDirectory);

        Console.WriteLine("=== CREATING CONNCETION ===");
        // Create connection
        if (File.Exists(_dbPath) && new FileInfo(_dbPath).Length == 0)
        {
            Console.WriteLine("Corrupted DB detected — deleting...");
            File.Delete(_dbPath);
        }
        Connection = new SQLiteAsyncConnection(_dbPath);

        Console.WriteLine("=== CONNCETION ESTABLISHED ===");


        await Connection.CreateTableAsync<__Init__>();


        Console.WriteLine("PRAGMA 1");
        await Connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
        Console.WriteLine("PRAGMA 2");
        await Connection.ExecuteAsync("PRAGMA busy_timeout = 2000;");
        Console.WriteLine("PRAGMA 3");

        
        Console.WriteLine("=== CREATING TABLES ===");
        // Create tables
        Console.WriteLine("Creating Virtue table...");
        await Connection.CreateTableAsync<Virtue>();
        Console.WriteLine("Virtue table created.");

        Console.WriteLine("Creating Meaning table...");
        await Connection.CreateTableAsync<Meaning>();
        Console.WriteLine("Meaning table created.");

        await Connection.CreateTableAsync<MeaningText>();
        await Connection.CreateTableAsync<Quote>();
        await Connection.CreateTableAsync<QuoteText>();

        Console.WriteLine("=== SEEDING IF EMPTY ===");
        // Seed if empty
        await SeedIfEmptyAsync();

        // Optional: run a health check and log it
        var health = await CheckHealthAsync();
        Console.WriteLine($"DB Healthy: {health.IsHealthy}");
        Console.WriteLine($"Virtues: {health.VirtueCount}");
        Console.WriteLine($"Meanings: {health.MeaningCount}");
        Console.WriteLine($"Quotes: {health.QuoteCount}");

        if (!health.IsHealthy)
        {
            foreach (var e in health.Errors)
                Console.WriteLine("DB ERROR: " + e);
        }
    }
    private async Task SeedIfEmptyAsync()
    {
        var count = await Connection.Table<Virtue>().CountAsync();
        if (count > 0)
            return;

        await SeedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        Console.WriteLine("RESET CALLED");
        IsReady = false;

        // Close connection if needed
        if (Connection != null)
            await Connection.CloseAsync();

        // Recreate connection (ensures no stale locks)
        Connection = new SQLiteAsyncConnection(_dbPath);
        await Connection.ExecuteAsync("PRAGMA foreign_keys = ON;");

        // Drop tables
        await Connection.DropTableAsync<Virtue>();
        await Connection.DropTableAsync<Meaning>();
        await Connection.DropTableAsync<MeaningText>();
        await Connection.DropTableAsync<Quote>();
        await Connection.DropTableAsync<QuoteText>();

        // Recreate tables
        await Connection.CreateTableAsync<Virtue>();
        await Connection.CreateTableAsync<Meaning>();
        await Connection.CreateTableAsync<MeaningText>();
        await Connection.CreateTableAsync<Quote>();
        await Connection.CreateTableAsync<QuoteText>();

        // Seed fresh data
        await SeedAsync();

        IsReady = true;
    }
    private async Task<string> LoadSeedJsonAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("virtues.json");
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    private async Task SeedAsync()
    {
        var json = await LoadSeedJsonAsync();
        var data = JsonSerializer.Deserialize<RootSeed>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? throw new Exception("Failed to deserialize seed JSON.");
        if (data.Virtues == null || data.Virtues.Count == 0)
            throw new Exception("Seed JSON contains no virtues.");

        await Connection.RunInTransactionAsync(conn =>
        {
            foreach (var v in data.Virtues)
            {
                conn.InsertOrReplace(new Virtue
                {
                    Id = v.Id,
                    Name = v.Name
                });

                foreach (var m in v.Meanings)
                {
                    conn.InsertOrReplace(new Meaning
                    {
                        Id = m.Id,
                        VirtueId = v.Id
                    });

                    foreach (var mt in m.Texts)
                    {
                        conn.InsertOrReplace(new MeaningText
                        {
                            Id = mt.Id,
                            MeaningId = m.Id,
                            LanguageCode = mt.Language,
                            Text = mt.Text
                        });
                    }
                }

                foreach (var q in v.Quotes)
                {
                    conn.InsertOrReplace(new Quote
                    {
                        Id = q.Id,
                        VirtueId = v.Id,
                        Author = q.Author
                    });

                    foreach (var qt in q.Texts)
                    {
                        conn.InsertOrReplace(new QuoteText
                        {
                            Id = qt.Id,
                            QuoteId = q.Id,
                            LanguageCode = qt.Language,
                            Text = qt.Text
                        });
                    }
                }
            }
        });
        Console.WriteLine("JSON LENGTH = " + json.Length);
    }
    public async Task<DatabaseHealthReport> CheckHealthAsync()
    {
        var report = new DatabaseHealthReport();

        // --- FILE CHECK ---------------------------------------------------------
        report.FileExists = File.Exists(_dbPath);
        if (report.FileExists)
            report.FileSizeBytes = new FileInfo(_dbPath).Length;
        else
            report.Errors.Add("Database file does not exist.");

        // --- TABLE COUNTS -------------------------------------------------------
        try
        {
            report.VirtueCount = await Connection.Table<Virtue>().CountAsync();
            report.MeaningCount = await Connection.Table<Meaning>().CountAsync();
            report.MeaningTextCount = await Connection.Table<MeaningText>().CountAsync();
            report.QuoteCount = await Connection.Table<Quote>().CountAsync();
            report.QuoteTextCount = await Connection.Table<QuoteText>().CountAsync();
        }
        catch (Exception ex)
        {
            report.Errors.Add("Table count failed: " + ex.Message);
            return report;
        }

        // --- BASIC VALIDATION ---------------------------------------------------
        if (report.VirtueCount == 0)
            report.Errors.Add("No virtues found.");

        if (report.MeaningCount == 0)
            report.Errors.Add("No meanings found.");

        if (report.QuoteCount == 0)
            report.Errors.Add("No quotes found.");

        // --- FOREIGN KEY VALIDATION --------------------------------------------
        var orphanMeanings = await Connection.QueryAsync<Meaning>(
            "SELECT * FROM Meaning WHERE VirtueId NOT IN (SELECT Id FROM Virtue)");

        if (orphanMeanings.Count > 0)
            report.Errors.Add($"Found {orphanMeanings.Count} orphan meanings.");

        var orphanMeaningTexts = await Connection.QueryAsync<MeaningText>(
            "SELECT * FROM MeaningText WHERE MeaningId NOT IN (SELECT Id FROM Meaning)");

        if (orphanMeaningTexts.Count > 0)
            report.Errors.Add($"Found {orphanMeaningTexts.Count} orphan meaning texts.");

        var orphanQuotes = await Connection.QueryAsync<Quote>(
            "SELECT * FROM Quote WHERE VirtueId NOT IN (SELECT Id FROM Virtue)");

        if (orphanQuotes.Count > 0)
            report.Errors.Add($"Found {orphanQuotes.Count} orphan quotes.");

        var orphanQuoteTexts = await Connection.QueryAsync<QuoteText>(
            "SELECT * FROM QuoteText WHERE QuoteId NOT IN (SELECT Id FROM Quote)");

        if (orphanQuoteTexts.Count > 0)
            report.Errors.Add($"Found {orphanQuoteTexts.Count} orphan quote texts.");

        return report;
    }
}

public class __Init__
{
    [PrimaryKey]
    public int Id { get; set; }
}
