using System;
using System.IO;

namespace Limbs.Web.Repositories.Interfaces
{
    public interface IUserFiles
    {

        Uri UploadOrderFile(Stream file, string name);

    }
}
