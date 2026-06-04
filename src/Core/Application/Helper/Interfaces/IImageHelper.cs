namespace Application.Helper;

public interface IImageHelper
{
    Task<string> SaveBase64Image(string dataUrl, string subFolder, string imagePrefix);
    Task<string> SaveBase64ImageIfChanged(string dataUrl, string? existingImageUrl, string subFolder, string imagePrefix);
    Task DeleteImageAsync(string? imageUrl);
    Task<string> GetImageBase64(string imageUrl);
}
