using VirtueTracker.Interfaces;

namespace VirtueTracker;

public partial class App : Application
{
	public App(IDatabaseService databaseService)
    {
        InitializeComponent();

        // Ensure DB initialization runs before anything else
        Task.Run(async () => await databaseService.InitializeAsync());

        MainPage = new AppShell();
    }

}
