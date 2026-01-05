namespace Infrastructure.Services;

public class ImageHelper : IImageHelper
{
    private readonly IWebHostEnvironment _env;
    public ImageHelper(IWebHostEnvironment env)
    {
        _env = env;
    }
    public async Task<string> SaveBase64Image(string dataUrl, string subFolder)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(dataUrl))
                return null;


            // data:[mime];base64,xxx
            var match = Regex.Match(dataUrl, @"^data:(?<mime>image\/(jpeg|png|gif));base64,(?<data>.+)$");
            //var match = Regex.Match(dataUrl, @"data:(?<type>.+?);base64,(?<data>.+)");
            if (!match.Success)
                throw new ArgumentException("Invalid image base64 format");

            //var mimeType = match.Groups["type"].Value;
            var mimeType = match.Groups["mime"].Value;
            var base64 = match.Groups["data"].Value;

            var extension = mimeType switch
            {
                "image/webp" => ".webp",
                "image/jpeg" => ".jpg",
                //"image/jpg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                _ => ".bin"
            };

            //var bytes = Convert.FromBase64String(base64);
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(base64);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid base64 string.");
            }

            using var image = Image.Load(imageBytes);

            var fileName = $"category-{Guid.NewGuid()}{extension}";
            //string folderPath = Path.Combine(_env.WebRootPath, subFolder);
            string folderPath = Path.Combine(_env.ContentRootPath, subFolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, fileName);

            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            // return relative path for the client, e.g. "images/categories/xyz.jpg"
            var relativePath = Path.Combine(subFolder, fileName).Replace("\\", "/");
            return relativePath;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public async Task<string> GetImageBase64(string imageUrl)
    {
        string relativePath = imageUrl.TrimStart('/');
        string filePath = Path.Combine(_env.ContentRootPath, relativePath);

        if (!File.Exists(filePath))
            return null;

        var bytes = await File.ReadAllBytesAsync(filePath);
        var base64 = Convert.ToBase64String(bytes);

        // choose correct mime based on extension
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var mime = ext switch
        {
            ".webp" => "webp",
            ".jpg" => "jpeg",
            ".jpeg" => "jpeg",
            ".png" => "png",
            ".gif" => "gif",
            _ => "octet-stream"
        };

        return $"data:{mime};base64,{base64}";
    }
}
