namespace AndroidMessageHandlerMauiApp.Interfaces.Implmentations;

public class FileDownloadService : IFileDownloadService
{
    private readonly HttpClient client;
    private readonly int bufferSize = 409600;

    public FileDownloadService()
    {
        //DEBUG
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, errors) => true;
        client = new(httpClientHandler);
    }

    public async Task DownloadFileAsync(string identifier, string url, string destinationPath, CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            var httpResponse = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (httpResponse.IsSuccessStatusCode)
            {
                // Step 2 : Filename
                var fileName = Path.GetFileName(url);

                // Step 4 : Get total of data
                var tempFilePath = string.Format("{0}.tmp", destinationPath);

                var directory = Path.GetDirectoryName(tempFilePath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using var fileStream = OpenStream(tempFilePath);

                using var stream = await httpResponse.Content.ReadAsStreamAsync(token);
                var totalRead = 0L;
                var buffer = new byte[bufferSize];
                var isMoreDataToRead = true;

                do
                {
                    if (token.IsCancellationRequested)
                    {
                        await fileStream.FlushAsync(token);

                        fileStream.Close();

                        await fileStream.DisposeAsync();

                        throw new OperationCanceledException(token);
                    }

                    int read = 0;

                    if (isMoreDataToRead = (read = await stream.ReadAsync(buffer, token)) > 0)
                    {
                        await fileStream.WriteAsync(buffer.AsMemory(0, read), token);

                        totalRead += read;
                    }

                } while (isMoreDataToRead);

                await fileStream.FlushAsync(token);

                fileStream.Close();

                await fileStream.DisposeAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"error {e.Message}");

            throw;
        }
    }

    private static FileStream OpenStream(string path) => new FileStream(path, FileMode.OpenOrCreate);
}
