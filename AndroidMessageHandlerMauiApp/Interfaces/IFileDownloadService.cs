namespace AndroidMessageHandlerMauiApp.Interfaces;

public interface IFileDownloadService
{
    Task DownloadFileAsync(string identifier, string url, string destinationPath, CancellationToken token = default);
}
