using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherTimerFunction
{
    public class ModelToSave
    {
        public string City { get; set; }
        public int Temperature { get; set; }
        public int  Humidity { get; set; }
        public int Pressure { get; set; }
        public DateTime WeatherDate { get; set; }

    }
}
