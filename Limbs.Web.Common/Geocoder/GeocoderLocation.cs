using System.Data.Entity.Spatial;
using System.Globalization;
using System.Threading.Tasks;

namespace Limbs.Web.Common.Geocoder
{
    public class GeocoderLocation
    {
        public static async Task<DbGeography> GetPointAsync(string pointAddress)
        {
            //IGeocoder geocoder = new GoogleGeocoder() { ApiKey = ConfigurationManager.AppSettings["Google.Maps.Key"] };
            //var addresses = await geocoder.GeocodeAsync(pointAddress);
            //var addressesArray = addresses as Address[] ?? addresses.ToArray();
            //
            //if (addressesArray.Length != 1) return null;
            //
            //var address = addressesArray.First();
            //
            ////if() TODO (ale): check number
            return await Task.Run(() => GeneratePoint(0,0));
            //return GeneratePoint(address.Coordinates.Latitude, address.Coordinates.Longitude);
        }
        
        public static DbGeography GeneratePoint(double lat, double lng)
        {
            return DbGeography.PointFromText("POINT(" + lng.ToString("G17", CultureInfo.InvariantCulture) + " " + lat.ToString("G17", CultureInfo.InvariantCulture) + ")", 4326);
        }
    }
}