using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Limbs.Web.Models;
using Limbs.Web.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


namespace Limbs.Web.Controllers
{
    [Authorize]
    public class GoogleGeoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public double GetLatGoogle(String Address)
        {
            var address = String.Format("http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false", Address.Replace(" ", "+"));
            var result = new System.Net.WebClient().DownloadString(address);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(result);

            var lat = dict["results"][0]["geometry"]["location"]["lat"];

            return Convert.ToDouble(lat);
            // return jss.Deserialize<dynamic>(result);
        }

        public double GetLongGoogle(String Address)
        {
            var address = String.Format("http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false", Address.Replace(" ", "+"));
            var result = new System.Net.WebClient().DownloadString(address);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(result);

            var lng = dict["results"][0]["geometry"]["location"]["lng"];

            return Convert.ToDouble(lng);
            // return jss.Deserialize<dynamic>(result);
        }

        public static dynamic GetPointGoogle(String Address)
        {
            var address = String.Format("http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false", Address.Replace(" ", "+"));
            var result = new System.Net.WebClient().DownloadString(address);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(result);

            var lat = dict["results"][0]["geometry"]["location"]["lat"];
            var lng = dict["results"][0]["geometry"]["location"]["lng"];

            return lat + lng;
            // return jss.Deserialize<dynamic>(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
