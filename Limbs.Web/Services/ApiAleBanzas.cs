using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Limbs.Web.Services
{
    public static class Api
    {
        public static HttpClient GetHttpClient()
        {
            return new HttpClient(new RequestContentMd5Handler
            {
                InnerHandler = new HmacSigningHandler
                {
                    InnerHandler = new WebRequestHandler
                    {
                        AllowAutoRedirect = false,
                        UseProxy = false,
                        UseCookies = false
                    }
                }
            });
        }

        public static Uri ToAbsoluteUri(this string path, object queryStringValues = null)
        {
            var ub = new UriBuilder("http", "api.alebanzas.com.ar");
            var pathAndQueryParts = path.Split('?');
            ub.Path = string.Join("/", "api", pathAndQueryParts[0].TrimStart('/'));
            if (queryStringValues != null)
            {
                ub.Query = string.Join("&", (from pd in TypeDescriptor.GetProperties(queryStringValues).Cast<PropertyDescriptor>()
                                             let v = pd.GetValue(queryStringValues)
                                             select string.Format("{0}={1}", HttpUtility.UrlEncode(pd.Name), HttpUtility.UrlEncode(v != null ? v.ToString() : ""))));
            }
            return ub.Uri;
        }

        public static IEnumerable<MediaTypeFormatter> GetContentFormatters()
        {
            var jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            jsonMediaTypeFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.abservicios.entry+json"));
            jsonMediaTypeFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.abservicios.ref+json"));
            yield return jsonMediaTypeFormatter;
        }
    }

    public class HmacSigningHandler : DelegatingHandler
    {
        private readonly string appKey;
        private readonly string appSecret;
        private readonly ABServiciosSignatureBuilder hmacb;

        public HmacSigningHandler()
        {
            hmacb = new ABServiciosSignatureBuilder();
            appKey = new Guid(ConfigurationManager.AppSettings["AleBanzasAPIkey"]).ToString("N");
            appSecret = new Guid(ConfigurationManager.AppSettings["AleBanzasAPIsecret"]).ToString("N");
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            request.Headers.Date = DateTime.UtcNow;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.abservicios.entry+json"));
            string signature = hmacb.GetSignature(appSecret, request);

            request.Headers.Authorization = new AuthenticationHeaderValue("ABS-H", string.Format("{0}:{1}", appKey, signature));
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class ABServiciosSignatureBuilder
    {
        private IEnumerable<string> GetCanonicalParts(HttpRequestMessage request)
        {
            HttpRequestHeaders httpRequestHeaders = request.Headers;
            HttpContentHeaders httpContentHeaders = request.Content != null ? request.Content.Headers : null;
            yield return request.Method.Method;
            yield return httpContentHeaders != null && httpContentHeaders.ContentMD5 != null ? Convert.ToBase64String(httpContentHeaders.ContentMD5) : string.Empty;
            yield return httpContentHeaders != null && httpContentHeaders.ContentType != null ? httpContentHeaders.ContentType.MediaType : string.Empty;
            yield return httpRequestHeaders.Date.HasValue ? httpRequestHeaders.Date.Value.ToString("r", CultureInfo.InvariantCulture) : string.Empty;
            IOrderedEnumerable<KeyValuePair<string, string>> customValues = httpRequestHeaders
                .Where(x => x.Key.StartsWith("X-ABS", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => new KeyValuePair<string, string>(x.Key.ToLowerInvariant().Trim(), string.Join(",", x.Value.Select(v => v.Trim()))))
                .OrderBy(x => x.Key);
            foreach (var customValue in customValues)
            {
                yield return string.Format("{0}:{1}", customValue.Key, customValue.Value);
            }
            yield return request.RequestUri.PathAndQuery;
        }

        public string GetSignature(string secret, HttpRequestMessage request)
        {
            string canonicalizedMessage = string.Join("\n", GetCanonicalParts(request));
            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(canonicalizedMessage))
            {
                return "";
            }
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
            byte[] valueBytes = Encoding.UTF8.GetBytes(canonicalizedMessage);
            string signature;

            using (var hmac = new HMACSHA256(secretBytes))
            {
                byte[] hash = hmac.ComputeHash(valueBytes);
                signature = Convert.ToBase64String(hash);
            }
            return signature;
        }
    }

    public class RequestContentMd5Handler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content == null)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            byte[] content = await request.Content.ReadAsByteArrayAsync();
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(content);
            request.Content.Headers.ContentMD5 = hash;
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }

    public class EntryCollection<T>
    {
        public EntryCollection()
        {
            _entries = new List<T>();
        }

        public IEnumerable<T> _entries { get; set; }
    }
}