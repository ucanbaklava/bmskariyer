using System.IO;

namespace Sinav.Business.Services.ImageServices
{
    public interface IImageService
    {
        string SaveImage(Stream file, string webrootpath, string path);
        string SaveImage(Stream file, string path);

        string Base64ToImage(byte[] bytes, string webrootpath, string path);
    }
}