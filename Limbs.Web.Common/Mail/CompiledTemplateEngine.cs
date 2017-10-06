using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using Microsoft.CSharp;

namespace Limbs.Web.Common.Mail
{
    public static class CompiledTemplateEngine
    {
        private interface ICompiledClassHolder
        {
            Assembly Assembly { get; }
            string ClassFullName { get; }
            object GetNewInstance();
        }

        private class CompiledClass<TModel> : ICompiledClassHolder
        {
            private readonly Lazy<Assembly> _compiledAssembly;
            private Func<object> _ctorDelegate;

            public CompiledClass(string classFullName, string template, string[] additionalNamespaceImports = null, string[] additionalReferencedAssemblies = null)
            {
                ClassFullName = classFullName;
                _compiledAssembly = new Lazy<Assembly>(() =>
                {
                    GeneratorResults razorTemplate = GenerateCode<TModel>(classFullName, template, additionalNamespaceImports);
                    return CreateCompiledAssemblyFor<TModel>(razorTemplate.GeneratedCode, additionalReferencedAssemblies);
                });
            }

            public Assembly Assembly => _compiledAssembly.Value;
            public string ClassFullName { get; }
            public object GetNewInstance()
            {
                if (_ctorDelegate == null)
                {
                    _ctorDelegate = Expression.Lambda<Func<object>>(Expression.New(Assembly.GetType(ClassFullName))).Compile();
                }
                return _ctorDelegate();
            }
        }

        private static readonly IDictionary<string, ICompiledClassHolder> CompiledTemplates = new ConcurrentDictionary<string, ICompiledClassHolder>();

        public static void Add<TModel>(string classFullName, string template, string[] additionalNamespaceImports = null, string[] additionalReferencedAssemblies = null)
        {
            if (string.IsNullOrWhiteSpace(classFullName))
            {
                throw new ArgumentNullException(nameof(classFullName));
            }
            if (classFullName.LastIndexOf('.') <= 0)
            {
                throw new ArgumentException("Clase sin namespace", nameof(classFullName));
            }
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            try
            {
                CompiledTemplates.Add(classFullName, new CompiledClass<TModel>(classFullName, template, additionalNamespaceImports, additionalReferencedAssemblies));
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("Existe ya un template registrado para " + classFullName, nameof(classFullName), e);
            }
        }

        public static void Remove(string classFullName)
        {
            if (string.IsNullOrWhiteSpace(classFullName))
            {
                throw new ArgumentNullException(nameof(classFullName));
            }
            if (classFullName.LastIndexOf('.') <= 0)
            {
                throw new ArgumentException("Clase sin namespace", nameof(classFullName));
            }
            if (CompiledTemplates.ContainsKey(classFullName))
            {
                CompiledTemplates.Remove(classFullName);
            }
        }

        private static GeneratorResults GenerateCode<TModel>(string classFullName, string template, IEnumerable<string> additionalNamespaceImports)
        {
            var lastIndexOf = classFullName.LastIndexOf('.');
            string classNamespace = classFullName.Substring(0, lastIndexOf);
            string className = classFullName.Substring(lastIndexOf + 1);

            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language)
            {
                DefaultBaseClass = typeof(RazorGeneratorBase<TModel>).FullName,
                DefaultClassName = className,
                DefaultNamespace = classNamespace,
            };
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Text");
            host.NamespaceImports.Add("System.Linq");
            host.NamespaceImports.Add(typeof(TModel).Namespace ?? throw new InvalidOperationException());
            if (additionalNamespaceImports != null)
            {
                foreach (var additionalNamespaceImport in additionalNamespaceImports)
                {
                    host.NamespaceImports.Add(additionalNamespaceImport);
                }
            }
            var engine = new RazorTemplateEngine(host);

            var tr = new StringReader(template); // here is where the string come in place
            GeneratorResults razorTemplate = engine.GenerateCode(tr);
            return razorTemplate;
        }

        private static Assembly CreateCompiledAssemblyFor<TModel>(CodeCompileUnit unitToCompile, IEnumerable<string> additionalReferencedAssemblies)
        {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add(typeof(RazorGeneratorBase<>).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(TModel).Assembly.Location);
            if (additionalReferencedAssemblies != null)
            {
                foreach (var additionalReferencedAssembly in additionalReferencedAssemblies)
                {
                    compilerParameters.ReferencedAssemblies.Add(additionalReferencedAssembly);
                }
            }
            compilerParameters.GenerateInMemory = true;

            CompilerResults compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);
            if (compilerResults.Errors.Count > 0)
            {
                var message = new StringBuilder(2048);
                foreach (CompilerError err in compilerResults.Errors)
                {
                    message.AppendFormat("Line:{0} Column:{1} Error:{2}\n", err.Line, err.Column, err.ErrorText);
                }
                throw new ArgumentException(message.ToString());
            }
            Assembly compiledAssembly = compilerResults.CompiledAssembly;
            return compiledAssembly;
        }

        public static string Render<TModel>(string classFullName, TModel model)
        {
            if (!CompiledTemplates.TryGetValue(classFullName, out var cc))
                throw new ArgumentException($"La clase {classFullName} no tiene templates asociados", nameof(classFullName));

            var templateInstance = (RazorGeneratorBase<TModel>)cc.GetNewInstance();
            templateInstance.DynModel = model;
            templateInstance.Model = model;
            return templateInstance.GetContent();
        }
    }
}
