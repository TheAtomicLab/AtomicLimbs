using System;
using System.Data.Entity.Spatial;
using System.Net;
using System.Web.Script.Serialization;

namespace Limbs.Web.Helpers
{
    public class Geolocalization
    {
        public static DbGeography GetPoint(string pointAddress)
        {
            var url = $"http://maps.google.com/maps/api/geocode/json?address={pointAddress.Replace(" ", "+")}&sensor=false";
            var result = new WebClient().DownloadString(url);
            var dict = new JavaScriptSerializer().Deserialize<dynamic>(result);
            var dictStatus = dict["status"];

            if (dictStatus != "OK") return GeneratePoint(0,0);

            var lat = double.Parse(dict["results"][0]["geometry"]["location"]["lat"].ToString());
            var lng = double.Parse(dict["results"][0]["geometry"]["location"]["lng"].ToString());

            return GeneratePoint(lat, lng);
        }

        public static bool PointIsValid(double? lat, double? Long)
        {
            return lat != 0 && Long != 0;
        }

       /*
        public async Task<JsonResult> GetPoint(string address)
        {
            var httpClient = Api.GetHttpClient();
            var result = await httpClient.GetAsync(("Geocoder/").ToAbsoluteUri(new { address = address }));
            var value = await result.Content.ReadAsStringAsync();
            var r = JsonConvert.DeserializeObject<List<GeocoderResult>>(value);

            return Json(r, JsonRequestBehavior.AllowGet);
        }
        */
        public static DbGeography GeneratePoint(double lat, double lng)
        {
            return DbGeography.FromText("POINT(" + lng +" " + lat +")");
        }
    }
}