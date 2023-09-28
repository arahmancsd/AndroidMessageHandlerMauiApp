using AndroidMessageHandlerMauiApp.Interfaces;

namespace AndroidMessageHandlerMauiApp;

public partial class MainPage : ContentPage
{
    private readonly IFileDownloadService _fileDownloadService;
    private readonly string BaseUrl = "https://www.azee.tech/Dari";

    public MainPage(IFileDownloadService fileDownloadService)
    {
        InitializeComponent();
        _fileDownloadService = fileDownloadService;
    }

    private async void Download_Click(object sender, EventArgs e)
    {
        var fileName = "001.mp3";
        var downloadLink = $"https://download.tvquran.com/download/TvQuran.com__Alsdes/{fileName}";

        try
        {
            await _fileDownloadService.DownloadFileAsync(Guid.NewGuid().ToString(), downloadLink,
                Path.Combine(FileSystem.Current.AppDataDirectory, fileName), default);

            await Shell.Current.DisplayAlert("Download Status", "Download Completed", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("error", ex.Message, "OK");
        }
    }

    private async void DownloadAll_Click(object sender, EventArgs e)
    {
        var fileNames = new List<string>()
        {
            "001.mp3",
            "114.mp3",
            "113.mp3",
            "112.mp3",
            "111.mp3",
            "110.mp3",
            "109.mp3",
            "108.mp3",
            "107.mp3",
            "106.mp3",
        };

        var destinationBasePath = Path.Combine(FileSystem.Current.AppDataDirectory);
        try
        {
            var maxParallelDegree = Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75));
            ParallelOptions parallelOptions = new()
            {
                CancellationToken = new CancellationToken(),
                MaxDegreeOfParallelism = maxParallelDegree
            };

            await Parallel.ForEachAsync(fileNames, parallelOptions, async (fileName, tk) =>
            {
                tk.ThrowIfCancellationRequested();

                var downloadLink = $"https://download.tvquran.com/download/TvQuran.com__Alsdes/{fileName}";

                await _fileDownloadService.DownloadFileAsync(Guid.NewGuid().ToString(), downloadLink,
                 Path.Combine(FileSystem.Current.AppDataDirectory, fileName), tk);
            });

            await Shell.Current.DisplayAlert("Download Status", "Download Completed", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("error", ex.Message, "OK");
        }
    }
}