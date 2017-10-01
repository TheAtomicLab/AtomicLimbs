using System;
using System.Data.Entity.Spatial;
using System.Web.Script.Serialization;

namespace Limbs.Web.Helpers
{
    public class Geolocalization
    {
        public static string GetPointGoogle(String PointAddress)
        {
            var pointAddress = String.Format("http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false", PointAddress.Replace(" ", "+"));
            var result = new System.Net.WebClient().DownloadString(pointAddress);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(result);
            var dictStatus = dict["status"];
            if (dictStatus == "OK")
            { 

            var lat = dict["results"][0]["geometry"]["location"]["lat"];
            var lng = dict["results"][0]["geometry"]["location"]["lng"];

            var latlng = Convert.ToString(lat).Replace(',', '.') + ',' + Convert.ToString(lng).Replace(',', '.');
            return latlng;
            }
            else
            {
                //Ver como hacerlo de forma linda aca
                return "0,0";
            }
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
            return DbGeography.FromText("POINT(" + lat +" " + lng +")");
        }
    }
}