namespace Application.Helper;

public interface IImageHelper
{
    Task<string> SaveBase64Image(string dataUrl, string subFolder);
    Task<string> GetImageBase64(string imageUrl);
}
