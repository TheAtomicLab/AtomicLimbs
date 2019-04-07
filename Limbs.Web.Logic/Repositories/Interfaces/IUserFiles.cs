using System;
using System.IO;

namespace Limbs.Web.Logic.Repositories.Interfaces
{
    public interface IUserFiles
    {
        Uri UploadOrderFile(Stream file, string name);
    }
}
