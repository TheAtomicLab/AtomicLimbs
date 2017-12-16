using System.Configuration;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Limbs.Web.Common.Geocoder.Google;

namespace Limbs.Web.Common.Geocoder
{
    public class GeocoderLocation
    {
        public static async Task<Address> GetAddressAsync(string pointAddress)
        {
            var geocoder = new GoogleGeocoder { ApiKey = ConfigurationManager.AppSettings["Google.Maps.Key"] };
            var addresses = await geocoder.GeocodeAsync(pointAddress);
            var addressesArray = addresses as GoogleAddress[] ?? addresses.ToArray();

            return addressesArray.Length == 0 ? null : addressesArray.First();
        }

        public static async Task<DbGeography> GeneratePointAsync(string pointAddress)
        {
            var address = await GetAddressAsync(pointAddress);

            return address == null ? null : GeneratePoint(address);
        }

        public static DbGeography GeneratePoint(Address address)
        {
            return address == null ? null : GeneratePoint(address.Coordinates.Latitude, address.Coordinates.Longitude);
        }

        public static DbGeography GeneratePoint(string[] point)
        {
            return (point == null || point.Length != 2 ) ? null : GeneratePoint(double.Parse(point[0]), double.Parse(point[1]));
        }

        public static DbGeography GeneratePoint(double lat, double lng)
        {
            return DbGeography.PointFromText("POINT(" + lng.ToString("G17", CultureInfo.InvariantCulture) + " " + lat.ToString("G17", CultureInfo.InvariantCulture) + ")", 4326);
        }
    }
}