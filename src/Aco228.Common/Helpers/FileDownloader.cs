using System.Net;
using Aco228.Common.Extensions;
using Aco228.Common.LocalStorage;

namespace Aco228.Common.Helpers;

public class FileDownloader : IDisposable
{
    private readonly IStorageManager _storageManager;
    private HttpClient _httpClient;
    private readonly IStorageFolder _tempFolder;

    public FileDownloader (IStorageManager storageManager)
    {
        _storageManager = storageManager;
        _httpClient = new HttpClient();
        _tempFolder = _storageManager.GetTempFolder();
    }

    public HttpClient HttpClient => _httpClient;

    public void SetClientHeader(string headerName, string headerValue)
    {
        _httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
    }

    public void SetProxy(string host, string username, string password)
    {
        var proxy = new WebProxy
        {
            Address = new Uri(host),
            BypassProxyOnLocal = false,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(username, password),
        };
        var httpClientHandler = new HttpClientHandler { Proxy = proxy, UseProxy = true, };
        httpClientHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        _httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: true);

        // var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
        // _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
    }

    public async Task<FileInfo> DownloadFileInfo(string url, string directoryLocation = "", string fileName = "")
    {
        var targetFileName = StringUrlHelper.GetFileName(url);
        var extension = Path.GetExtension(targetFileName);

        if (string.IsNullOrEmpty(fileName))
            fileName = targetFileName;

        // Ensure file has proper extension
        if (!fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            fileName += extension;

        if (!string.IsNullOrEmpty(directoryLocation) && !Directory.Exists(directoryLocation))
            throw new ArgumentException($"Directory does not exist: {directoryLocation}");

        if (string.IsNullOrEmpty(directoryLocation))
            directoryLocation = _tempFolder.GetCurrentPath();

        // Handle local paths
        if (url.StartsWith(@"C:\") || url.StartsWith(@"C:/"))
        {
            return _tempFolder.CopyFile(url, IdHelper.GetId("localhost_file"));
        }

        if (url.StartsWith("http://localhost") || url.StartsWith("https://localhost"))
        {
            return ReadFromLocalhost(url);
        }

        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            return ReadFromLocalhost(url);
        }

        // --- Download file from internet ---
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var fileLocation = Path.Combine(directoryLocation, fileName);

        // Ensure extension from content type if missing
        if (string.IsNullOrEmpty(Path.GetExtension(fileLocation)))
        {
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType == "image/jpeg") fileLocation += ".jpg";
        }

        // Generate unique name if file already exists
        if (File.Exists(fileLocation))
        {
            var uniqueName = IdHelper.GetId("dwn") + "_" + fileName;
            fileLocation = Path.Combine(directoryLocation, uniqueName);
        }

        // Write to a temp file first
        var tempFile = Path.Combine(directoryLocation, Path.GetRandomFileName());

        await using (var responseStream = await response.Content.ReadAsStreamAsync())
        await using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await responseStream.CopyToAsync(fileStream);
        }

        // Atomically move into place
        File.Move(tempFile, fileLocation, overwrite: true);

        var fileInfo = new FileInfo(fileLocation);
        if (!fileInfo.Exists)
            throw new IOException($"Error downloading file: {url}");

        return fileInfo;
    }

    private FileInfo ReadFromLocalhost(string url)
    {
        string searchedParam = url.Split("?")[0].GetUntilCharReverse('/');
        if (string.IsNullOrEmpty(searchedParam))
            throw new ArgumentException($"Could not find");

        var storageFileInfo = _storageManager.DeepSearchFor(searchedParam);
        if (storageFileInfo == null)
            throw new ArgumentException($"Could not find this file on localhost: {searchedParam}");

        return _tempFolder.CopyFile(storageFileInfo, IdHelper.GetId("localhost_file"));
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}