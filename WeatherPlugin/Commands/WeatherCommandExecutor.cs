using System.Linq;
using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;
using Shinkei.IRC;
using System.Net;
using Shinkei;
using System.Collections.Generic;
using System;
using System.Text;


namespace WeatherPlugin.Commands
{
    public class WeatherCommandExecutor : ICommandExecutor
    {

        private Dictionary<int, double> densityMap;

        public WeatherCommandExecutor()
        {
            densityMap = new Dictionary<int, double>();
            densityMap.Add(-18, 1.5);
            densityMap.Add(-15, 1.9);
            densityMap.Add(-12, 2.4);
            densityMap.Add(-09, 3.0);
            densityMap.Add(-07, 3.7);
            densityMap.Add(-04, 4.6);
            densityMap.Add(-01, 5.6);
            densityMap.Add( 02, 6.9);
            densityMap.Add( 04, 8.4);
            densityMap.Add( 07, 10.2);
            densityMap.Add( 10, 12.3);
            densityMap.Add( 13, 14.8);
            densityMap.Add( 16, 17.7);
            densityMap.Add( 18, 21.0);
            densityMap.Add( 21, 25.0);
            densityMap.Add( 24, 29.6);
            densityMap.Add( 27, 35.0);
            densityMap.Add( 29, 41.0);
            densityMap.Add( 32, 48.1);
            densityMap.Add( 35, 56.2);
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count <= 0)
            {
                return false;
            }
            string place = data.Arguments[0];

            data.SendResponse(GetWeatherForPlace(place));

            return true;
        }

        private string GetWeatherForPlace(string place)
        {
            place = place.Replace(" ", ",");
            string returnstring = "%CITY% " + 
                                    "" + "|" + "" +
                                    "" + " %TEMP%°C " + "" +
                                    "" + "|" + "" + 
                                    " %WEATHERINFO% " +
                                    "" + "|" + "" + 
                                    " H: %HUMIDITY%%, P: %PRESSURE%hPa";

            string ApiUrl = "http://api.openweathermap.org/data/2.5/weather?units=metric&q=";

            WebClient client = new WebClient();
            byte[] jsonbytes = client.DownloadData(ApiUrl + place);
            string jsonResponse = UTF8Encoding.UTF8.GetString(jsonbytes);

            Response response = JsonHelper.DeserializeFromString<WeatherResponse>(jsonResponse);
            if (response == default(WeatherResponse))
            {
                response = JsonHelper.DeserializeFromString<ErrorResponse>(jsonResponse);
            }

            if (response != default(WeatherResponse))
            {
                if (((WeatherResponse)response).cod == 200)
                {
                    returnstring = returnstring.Replace("%CITY%", ((WeatherResponse)response).name);
                    returnstring = returnstring.Replace("%TEMP%", ((WeatherResponse)response).main.temp.ToString());
                    returnstring = returnstring.Replace("%HUMIDITY%", ((WeatherResponse)response).main.humidity.ToString());
                    returnstring = returnstring.Replace("%PRESSURE%", ((WeatherResponse)response).main.pressure.ToString());

                    string weatherinfo = String.Empty;
                    foreach (WeatherResponse.Weather info in ((WeatherResponse)response).weather)
                    {
                        weatherinfo += info.description + ", ";
                    }
                    weatherinfo = weatherinfo.Trim(' ');
                    weatherinfo = weatherinfo.Trim(',');

                    returnstring = returnstring.Replace("%WEATHERINFO%", weatherinfo);

                    return returnstring;
                }
                else
                {
                    return ((ErrorResponse)response).message;
                }
            }

            return "Internal Error.";
        }

        public interface Response
        {

        }

        public class ErrorResponse : Response
        {
            public int cod { get; set; }
            public string message { get; set; }
        }

        public class WeatherResponse : Response
        {
            public class Coord
            {
                public double lon { get; set; }
                public double lat { get; set; }
            }

            public class Weather
            {
                public int id { get; set; }
                public string main { get; set; }
                public string description { get; set; }
                public string icon { get; set; }
            }

            public class Main
            {
                public double temp { get; set; }
                public double pressure { get; set; }
                public double humidity { get; set; }
                public double temp_min { get; set; }
                public double temp_max { get; set; }
            }

            public class Wind
            {
                public double speed { get; set; }
            }

            public class Clouds
            {
                public double all { get; set; }
            }

            public class Sys
            {
                public int type { get; set; }
                public int id { get; set; }
                public double message { get; set; }
                public string country { get; set; }
                public double sunrise { get; set; }
                public double sunset { get; set; }
            }

            public Coord coord { get; set; }
            public List<Weather> weather { get; set; }
            public string @base { get; set; }
            public Main main { get; set; }
            public double visibility { get; set; }
            public Wind wind { get; set; }
            public Clouds clouds { get; set; }
            public double dt { get; set; }
            public Sys sys { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }
        }
            
    }
}