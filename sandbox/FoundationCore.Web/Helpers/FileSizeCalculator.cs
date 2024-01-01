namespace FoundationCore.Web.Helpers
{
    public static class FileSizeCalculator
    {
        public static async Task<long> GetFileSize(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return 0;
            }

            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.Headers.ContentLength ?? 0;
            }
            return 0;
        }

        public static async Task<string> GetFileMimeType(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            }
            return string.Empty;
        }

        public static async Task<RemoteFileInfo> GetRemoteFileInfo(string url)
        {
            var fileInfo = new RemoteFileInfo();

            if (string.IsNullOrEmpty(url))
            {
                return fileInfo;
            }

            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                fileInfo.SizeInBytes = response.Content.Headers.ContentLength ?? 0;
                fileInfo.MimeType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            }
            return null;
        }
    }

    public class RemoteFileInfo
    {
        public string MimeType { get; set; }
        public long SizeInBytes { get; set; }
    }
}
