using System.Net;
using Aco228.Common.Extensions;
using Aco228.Common.LocalStorage;
using Aco228.Common.Models;

namespace Aco228.Common.Helpers;

public class FileDownloader : IDisposable, ITransient
{
    private readonly IStorageManager _storageManager;
    private HttpClient _httpClient;
    private IStorageFolder _defaultFolder;
    
    private static readonly Dictionary<string, string> _mediaTypeExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"]  = ".jpg",
        ["image/png"]   = ".png",
        ["image/webp"]  = ".webp",
        ["image/gif"]   = ".gif",
        ["image/bmp"]   = ".bmp",
        ["image/tiff"]  = ".tiff",
        ["image/avif"]  = ".avif",
        ["image/svg+xml"] = ".svg",
        ["application/pdf"] = ".pdf",
        ["video/mp4"]   = ".mp4",
        ["video/webm"]  = ".webm",
    };

    public FileDownloader (IStorageManager storageManager)
    {
        _storageManager = storageManager;
        var httpFactory = ServiceProviderHelper.GetService<IHttpClientFactory>();
        _httpClient = httpFactory != null ? httpFactory.CreateClient() :  new HttpClient();
        _defaultFolder = _storageManager.GetTempFolder();
    }

    public HttpClient HttpClient => _httpClient;
    
    public static FileDownloader Get()
        => new (StorageManager.Instance);

    public FileDownloader SetClient(HttpClient client)
    {
        _httpClient = client;
        return this;
    }

    public void SetClientHeader(string headerName, string headerValue)
    {
        _httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
    }

    public FileDownloader SetDefaultFolder(IStorageFolder folder)
    {
        _defaultFolder = folder;
        return this;
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
        // Handle local paths
        if (url.StartsWith(@"C:\") || url.StartsWith(@"C:/"))
            return _defaultFolder.CopyFile(url, IdHelper.GetId("localhost_file"));

        if (url.StartsWith("http://localhost") || url.StartsWith("https://localhost"))
            return ReadFromLocalhost(url);

        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            return ReadFromLocalhost(url);

        if (!string.IsNullOrEmpty(directoryLocation) && !Directory.Exists(directoryLocation))
            throw new ArgumentException($"Directory does not exist: {directoryLocation}");

        if (string.IsNullOrEmpty(directoryLocation))
            directoryLocation = _defaultFolder.GetCurrentPath();

        // --- Download file from internet ---
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        // 1. Try Content-Disposition filename
        if (string.IsNullOrEmpty(fileName))
        {
            var cd = response.Content.Headers.ContentDisposition;
            if (cd?.FileNameStar != null)
                fileName = cd.FileNameStar;
            else if (cd?.FileName != null)
                fileName = cd.FileName.Trim('"');
        }

        // 2. Fall back to URL filename
        if (string.IsNullOrEmpty(fileName))
            fileName = StringUrlHelper.GetFileName(url);

        // 3. Resolve extension - try URL/filename first, then Content-Type
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension))
        {
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType != null && _mediaTypeExtensions.TryGetValue(mediaType, out var detectedExt))
            {
                extension = detectedExt;
                fileName += extension;
            }
        }

        // Ensure fileName has the resolved extension
        if (!string.IsNullOrEmpty(extension) && !fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            fileName += extension;

        // 4. If we still have no usable fileName, generate one
        if (string.IsNullOrEmpty(fileName))
            fileName = IdHelper.GetId("dwn") + (extension ?? string.Empty);

        var fileLocation = Path.Combine(directoryLocation, IdHelper.GetId("dwn") + "_" + fileName);

        // Write to a temp file first, then atomically move into place
        var tempFile = Path.Combine(directoryLocation, Path.GetRandomFileName());

        await using (var responseStream = await response.Content.ReadAsStreamAsync())
        await using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await responseStream.CopyToAsync(fileStream);
        }

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

        return _defaultFolder.CopyFile(storageFileInfo, IdHelper.GetId("localhost_file"));
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}