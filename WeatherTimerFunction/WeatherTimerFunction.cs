using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace WeatherTimerFunction
{
    public static class WeatherTimerFunction
    {
       
        #region  POCO
        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }

        public class Sys
        {
            public int type { get; set; }
            public int id { get; set; }
            public double message { get; set; }
            public string country { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }
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
            public int temp { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public int temp_min { get; set; }
            public int temp_max { get; set; }
        }

        public class Wind
        {
            public int speed { get; set; }
            public int deg { get; set; }
        }

        public class Clouds
        {
            public int all { get; set; }
        }

        public class List
        {
            public Coord coord { get; set; }
            public Sys sys { get; set; }
            public List<Weather> weather { get; set; }
            public Main main { get; set; }
            public int visibility { get; set; }
            public Wind wind { get; set; }
            public Clouds clouds { get; set; }
            public int dt { get; set; }
            public int id { get; set; }
            public string name { get; set; }
        }

        public class RootObject
        {
            public int cnt { get; set; }
            public List<List> list { get; set; }
        }
        #endregion

        private static string _key;

        private static HttpClient _client;

        private static Uri _baseUri;

        private static string _connectionString;

        //constructor initialize static fields
        static WeatherTimerFunction()
        {
            //weather app key initializer
            _key = "";

            //base Uri initializer;
            _baseUri = new Uri("http://api.openweathermap.org/data/2.5/group?");

            //http client initializer
            _client = new HttpClient();
            _client.BaseAddress = _baseUri;

            _connectionString = "Server=(localdb)\\mssqllocaldb;Database=TemporaryStorage;Trusted_Connection=True;";
        }
        [FunctionName("WeatherTimerFunction")]
        public static void Run([TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            AppContext context = new AppContext();
            

            string requestUri = $"id=1526384,1526273,1518980&units=metric&appid={_key}";
            string jsonRes = _client.GetStringAsync(_baseUri + requestUri).Result;

            RootObject data = JsonConvert.DeserializeObject<RootObject>(jsonRes);
            DateTime weatherDate = DateTime.Now;
            ModelToSave city1 = new ModelToSave
            {
                City = data.list[0].name,
                Temperature = data.list[0].main.temp,
                Humidity = data.list[0].main.humidity,
                Pressure = data.list[0].main.pressure,
                WeatherDate = weatherDate
            };
            ModelToSave city2 = new ModelToSave
            {
                City = data.list[1].name,
                Temperature = data.list[1].main.temp,
                Humidity = data.list[1].main.humidity,
                Pressure = data.list[1].main.pressure,
                WeatherDate = weatherDate
            };
            ModelToSave city3 = new ModelToSave
            {
                City = data.list[0].name,
                Temperature = data.list[2].main.temp,
                Humidity = data.list[2].main.humidity,
                Pressure = data.list[2].main.pressure,
                WeatherDate = weatherDate
            };

            context.Add(city1);
            context.Add(city2);
            context.Add(city3);

            context.SaveChangesAsync();
        }

        public class AppContext : DbContext
        {
            public DbSet<ModelToSave> Weather { get; set; }

            public AppContext()
            {
                Database.EnsureCreated();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}
