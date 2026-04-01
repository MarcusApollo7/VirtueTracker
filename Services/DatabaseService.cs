using SQLite;
using VirtueTracker.Models;

namespace VirtueTracker.Services;
public class DatabaseService
    {
    private const string DbFileName = "Virtues.db";
    private readonly string _dbPath;
    public SQLiteAsyncConnection Connection { get; private set; } = default!;
    public bool IsReady { get; private set; }

    public DatabaseService()
    {
        Console.WriteLine("DATABASE SERVICE CREATED");
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, DbFileName);
        Console.WriteLine("DB PATH = " + _dbPath);
    }
    static DatabaseService()
    {
        SQLitePCL.Batteries_V2.Init();
    }


    public async Task InitializeAsync()
    {
        Console.WriteLine("INIT 1: Start");

        try
        {
            Directory.CreateDirectory(FileSystem.AppDataDirectory);

            Console.WriteLine("INIT 2: Creating SQLite connection...");
            Connection = new SQLiteAsyncConnection(_dbPath);
            await Connection.ExecuteAsync("PRAGMA busy_timeout = 2000;");

            Console.WriteLine("DB PATH = " + _dbPath);
            Console.WriteLine("DB EXISTS? " + File.Exists(_dbPath));
            if (File.Exists(_dbPath))
            {
                var info = new FileInfo(_dbPath);
                Console.WriteLine("DB SIZE = " + info.Length);
            }
            Console.WriteLine("INIT 2a: Checking DB health...");
            if (await IsDatabaseCorrupted())
            {
                Console.WriteLine("INIT 2b: DB is corrupted. Deleting...");
                File.Delete(_dbPath);
                Connection = new SQLiteAsyncConnection(_dbPath);
            }


            Console.WriteLine("INIT 3: Creating tables...");
            Console.WriteLine("INIT 3a: Creating Virtue table...");
            await Connection.CreateTableAsync<Virtue>();

            Console.WriteLine("INIT 3b: Creating Meaning table...");
            await Connection.CreateTableAsync<Meaning>();

            Console.WriteLine("INIT 3c: Creating MeaningText table...");
            await Connection.CreateTableAsync<MeaningText>();

            Console.WriteLine("INIT 3d: Creating Quote table...");
            await Connection.CreateTableAsync<Quote>();

            Console.WriteLine("INIT 3e: Creating QuoteText table...");
            await Connection.CreateTableAsync<QuoteText>();


            Console.WriteLine("INIT 4: Seeding if empty...");
            await SeedIfEmptyAsync();

            Console.WriteLine("INIT 5: END");
            IsReady = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("INIT ERROR: " + ex);
            throw;
        }
    }

    private async Task<bool> IsDatabaseCorrupted()
    {
        try
        {
            // Add a busy timeout so SQLite doesn't hang forever
            await Connection.ExecuteAsync("PRAGMA busy_timeout = 2000;");

            var version = await Connection.ExecuteScalarAsync<int>("PRAGMA schema_version;");
            Console.WriteLine("DB schema version = " + version);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("DB CORRUPTION DETECTED: " + ex.Message);
            return true;
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
        /* IsReady = false;

        // Close connection if needed
        if (Connection != null)
            await Connection.CloseAsync();

        // Recreate connection (ensures no stale locks)
        Connection = new SQLiteAsyncConnection(_dbPath);
        await Connection.ExecuteAsync("PRAGMA foreign_keys = ON;");

        // Drop tables
        await Connection.DropTableAsync<VirtueObject>();
        await Connection.DropTableAsync<MeaningObject>();
        await Connection.DropTableAsync<MeaningText>();
        await Connection.DropTableAsync<QuoteObject>();
        await Connection.DropTableAsync<QuoteText>();

        // Recreate tables
        await Connection.CreateTableAsync<VirtueObject>();
        await Connection.CreateTableAsync<MeaningObject>();
        await Connection.CreateTableAsync<MeaningText>();
        await Connection.CreateTableAsync<QuoteObject>();
        await Connection.CreateTableAsync<QuoteText>();

        // Seed fresh data
        await SeedAsync();

        IsReady = true; */
    }
    private async Task SeedAsync()
    {
        // --- VIRTUES ---------------------------------------------------------
        var courage = new Virtue
        {
            Id = "virtue_courage",
            Name = "Courage"
        };

        await Connection.InsertAsync(courage);

        // --- MEANINGS --------------------------------------------------------
        var meaning1 = new Meaning
        {
            Id = "meaning_courage_1",
            VirtueId = courage.Id
        };

        var meaning2 = new Meaning
        {
            Id = "meaning_courage_2",
            VirtueId = courage.Id
        };

        await Connection.InsertAsync(meaning1);
        await Connection.InsertAsync(meaning2);

        // --- MEANING TEXTS ---------------------------------------------------
        await Connection.InsertAsync(new MeaningText
        {
            Id = "meaningtext_courage_1_en",
            MeaningId = meaning1.Id,
            LanguageCode = "en",
            Text = "Courage is the willingness to act despite fear."
        });

        await Connection.InsertAsync(new MeaningText
        {
            Id = "meaningtext_courage_2_en",
            MeaningId = meaning2.Id,
            LanguageCode = "en",
            Text = "Courage is choosing what is right even when it is difficult."
        });

        // --- QUOTES ----------------------------------------------------------
        var quote1 = new Quote
        {
            Id = "quote_courage_1",
            VirtueId = courage.Id,
            Author = "Nelson Mandela"
        };

        var quote2 = new Quote
        {
            Id = "quote_courage_2",
            VirtueId = courage.Id,
            Author = "Mark Twain"
        };

        var quote3 = new Quote
        {
            Id = "quote_courage_3",
            VirtueId = courage.Id,
            Author = "Winston Churchill"
        };

        await Connection.InsertAsync(quote1);
        await Connection.InsertAsync(quote2);
        await Connection.InsertAsync(quote3);

        // --- QUOTE TEXTS -----------------------------------------------------
        await Connection.InsertAsync(new QuoteText
        {
            Id = "quotetext_courage_1_en",
            QuoteId = quote1.Id,
            LanguageCode = "en",
            Text = "Courage is not the absence of fear, but the triumph over it."
        });

        await Connection.InsertAsync(new QuoteText
        {
            Id = "quotetext_courage_2_en",
            QuoteId = quote2.Id,
            LanguageCode = "en",
            Text = "Courage is resistance to fear, mastery of fear—not absence of fear."
        });

        await Connection.InsertAsync(new QuoteText
        {
            Id = "quotetext_courage_3_en",
            QuoteId = quote3.Id,
            LanguageCode = "en",
            Text = "Success is not final, failure is not fatal: it is the courage to continue that counts."
        });
    }
}