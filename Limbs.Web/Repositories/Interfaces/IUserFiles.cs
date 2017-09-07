using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limbs.Web.Repositories.Interfaces
{
    interface IUserFiles
    {

        Uri UploadOrderFile(Stream file, string name);

    }
}
