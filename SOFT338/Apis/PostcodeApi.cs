using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace SOFT338.Apis
{
    public class PostcodeApi
    {
        public static string BaseUrl = "http://api.postcodes.io/postcodes";

        public static string GetPostcodeFromLatLong(double latitude, double longitude)
        {
            string query = String.Format("?lon={0}&lat={1}", longitude, latitude);
            WebRequest request = WebRequest.Create(PostcodeApi.BaseUrl + query);
            WebResponse response = request.GetResponse();

            if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            // God damn this is ugly
            // We're creating a dummy object with a property "result" which is an array of other objects
            var dummyObject = new { result = new[] { new { postcode = string.Empty } } };
            var json = JsonConvert.DeserializeAnonymousType(reader.ReadToEnd(), dummyObject);

            return json.result[0].postcode;
        }
    }
}