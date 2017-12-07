using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web.Compilation;

namespace Limbs.Web.Common.Resources
{
    public sealed class ValidationResourceProviderFactory : System.Web.Compilation.ResourceProviderFactory
    {
        public ValidationResourceProviderFactory()
        {
        }

        public override IResourceProvider CreateGlobalResourceProvider(string classKey)
        {
            return new GlobalResourceProvider(classKey);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            throw new NotImplementedException("Local resources are not supported yet");
        }
    }

    public class GlobalResourceProvider : IResourceProvider
    {
        public GlobalResourceProvider(string classKey)
        {
            if(string.IsNullOrEmpty(classKey)) throw new InvalidOperationException(nameof(classKey));

            var type = Type.GetType(classKey, false);
            if (type == null)
            {
                var asmName = classKey;
                while (asmName.IndexOf(".", StringComparison.Ordinal) > -1 && type == null)
                {
                    asmName = asmName.Substring(0, asmName.LastIndexOf(".", StringComparison.Ordinal));
                    type = Type.GetType(classKey + "," + asmName, false);
                }
            }

            if(type == null) throw new InvalidOperationException(nameof(type));

            Manager = CreateResourceManager(classKey, type.Assembly);
        }

        public ResourceManager Manager { get; set; }

        #region IResourceProvider implementation

        public IResourceReader ResourceReader { get; set; }

        public object GetObject(string resourceKey, CultureInfo culture)
        {
            return Manager.GetObject(resourceKey, culture);
        }

        #endregion

        private ResourceManager CreateResourceManager(string classKey, Assembly assembly)
        {
            return new ResourceManager(classKey, assembly);
        }
    }
}
