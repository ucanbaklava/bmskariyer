using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Sinav.Business.Services.ImageServices
{
    public class ImageService: IImageService
    {
        private readonly ILogger<ImageService> _logger;

        public ImageService( ILogger<ImageService> logger)
        {
            _logger = logger;
        }
        public string SaveImage(Stream file, string webrootpath, string path)
        {
            var fileStream = new FileStream(Path.Combine(webrootpath,path), FileMode.Create, FileAccess.Write);
            var extension = Path.GetExtension(fileStream.Name);
            file.Seek(0, SeekOrigin.Begin);
            file.CopyTo(fileStream);
            fileStream.Dispose();

            return Path.Combine("https://bmskariyer.com/", path);
        }
        public string SaveImage(Stream file, string path)
        {
            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            var extension = Path.GetExtension(fileStream.Name);
            file.Seek(0, SeekOrigin.Begin);
            file.CopyTo(fileStream);
            fileStream.Dispose();

            return Path.Combine("\\", path);
        }

        public string Base64ToImage(byte[] bytes, string webrootpath, string path)
        {
            try
            {
                var fPath = Path.Combine(webrootpath, path);
                using (var imageFile = new FileStream(fPath, FileMode.Create))
                {
                    imageFile.Write(bytes ,0, bytes.Length);
                    imageFile.Flush();
                }

                
                return Path.Combine("\\",path);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Kayıt sırasında resim yüklenirken hata");
                return "";
            }
        }
        
    }
}