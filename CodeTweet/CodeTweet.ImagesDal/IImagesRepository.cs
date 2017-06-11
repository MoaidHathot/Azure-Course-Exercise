using System;
using System.IO;
using System.Threading.Tasks;

namespace CodeTweet.ImagesDal
{
    public interface IImagesRepository
    {
        Task<string> UploadImageAsync(string id, Stream stream);
    }
}
