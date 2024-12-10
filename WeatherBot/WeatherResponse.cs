using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WeatherBot
{

    public class WeatherResponse
    {
        public TemperatureInfo Main { get; set; }

        public string Name { get; set; }

    }
    public class WeatherForecastResponse
    {
        [JsonProperty("list")]
        public List<ForecastItem> List { get; set; }
    }
    public class ForecastItem
    {
        public long Dt { get; set; }  // Время в Unix-формате
        public TemperatureInfo Main { get; set; }
        public float Temp { get; set; }
        public float Temp_Min { get; set; }
        public float Temp_Max { get; set; }
        public float Feels_Like { get; set; }
        public List<WeatherInfo> Weather { get; set; }
    }
    public class WeatherInfo
    {
        public string Description { get; set; }
    }
}
