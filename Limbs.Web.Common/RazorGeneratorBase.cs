using System;
using System.Text;
using System.Web;
using Limbs.Web.Common.Mail;
using Microsoft.CSharp.RuntimeBinder;

namespace Limbs.Web.Common
{
    public abstract class RazorGeneratorBase<TModel>
    {
        private StringBuilder buffer;

        /// <summary>
        /// This is just a custom property
        /// </summary>
        public dynamic DynModel { get; set; }

        /// <summary>
        /// This is just a custom property
        /// </summary>
        public TModel Model { get; set; }

        /// <summary>
        /// This method is required and have to be exactly as declared here.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// This method is required and can be public but have to have exactly the same signature
        /// </summary>
        protected void Write(object value)
        {
            WriteLiteral(HttpUtility.HtmlEncode(value));
        }

        /// <summary>
        /// This method is required and can be public but have to have exactly the same signature
        /// </summary>
        protected void WriteLiteral(object value)
        {
            buffer.Append(value);
        }

        /// <summary>
        /// This method is required and can be public but have to have exactly the same signature
        /// </summary>
        public virtual void WriteAttribute(string name, Tuple<string, int> startTag, Tuple<string, int> endTag, params object[] values)
        {
            WriteLiteral(startTag.Item1);

            // All values must be of type: Tuple<Tuple<string, int>, Tuple<X, int>, bool>
            foreach (dynamic value in values)
            {
                try
                {
                    WriteAttribute(value);
                }
                catch (RuntimeBinderException)
                {
                    //This should never happen
                }
            }
            WriteLiteral(endTag.Item1);
        }

        private void WriteAttribute<T>(Tuple<Tuple<string, int>, Tuple<T, int>, bool> value)
        {
            if (value != null)
            {
                Write(value.Item1.Item1);
                Write(value.Item2.Item1);
            }
        }
        /// <summary>
        /// This method is required and can be public but have to have exactly the same signature
        /// </summary>
        protected virtual object Partial<TPModel>(string className, TPModel model)
        {
            return new HtmlString(CompiledTemplateEngine.Render(className, model));
        }

        protected virtual object Raw(string content)
        {
            return new HtmlString(content);
        }

        /// <summary>
        /// This method is just to have the rendered content without call Execute.
        /// </summary>
        /// <returns>The rendered content.</returns>
        public string GetContent()
        {
            buffer = new StringBuilder(20480);
            Execute();
            return buffer.ToString();
        }
    }
}
