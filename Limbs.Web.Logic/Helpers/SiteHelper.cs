using System.Data.Entity.Spatial;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Limbs.Web.Logic.Helpers
{
    public static class SiteHelper
    {
        public static async Task<HttpResponseMessage> PostXmlRequestAsync(string baseUrl, string xmlString, string action)
        {
            using (var httpClient = new HttpClient())
            {
                var httpContent = new StringContent(xmlString, Encoding.UTF8, "text/xml");
                httpContent.Headers.Add("SOAPAction", action);

                return await httpClient.PostAsync(baseUrl, httpContent);
            }
        }

        public static DbGeography GeneratePoint(string[] point)
        {
            return (point == null || point.Length != 2) ? null : GeneratePoint(double.Parse(point[0]), double.Parse(point[1]));
        }

        private static DbGeography GeneratePoint(double lat, double lng)
        {
            return DbGeography.PointFromText("POINT(" + lng.ToString("G17", CultureInfo.InvariantCulture) + " " + lat.ToString("G17", CultureInfo.InvariantCulture) + ")", 4326);
        }
    }
}
