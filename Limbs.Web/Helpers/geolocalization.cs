using Limbs.Web.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
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

        public static bool PointIsValid(double Lat,double Long)
        {
            if(Lat == 0 && Long == 0)
            {
                //is not valid. Because the lat and long is saved with "0,0" in the register.
                return false;
            }
            else
            {
                return true;
            }
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
    }
}