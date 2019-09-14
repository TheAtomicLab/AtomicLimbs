using System;
using System.IO;
using System.Threading.Tasks;

namespace Limbs.Web.Logic.Repositories.Interfaces
{
    public interface IUserFiles
    {
        Uri UploadOrderFile(Stream file, string name, string containerName = null);
        Task<bool> RemoveImageAsync(string imageId, string containerName = null);
    }
}