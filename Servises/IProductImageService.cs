namespace Rozetka.Servises
{
    public interface IProductImageService
    {
        Task AddImagesAsync(int productId, IEnumerable<ImageData> images);
    }

    public class ImageData
    {
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        
    }
}
