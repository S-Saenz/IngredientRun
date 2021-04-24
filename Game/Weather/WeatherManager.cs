using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace WillowWoodRefuge
{
    public class WeatherManager
    {
        // time of day
        protected bool isNight;

        // weather
        //public Dictionary<string, string> weather;
        protected string weather;

        //constructor
        public WeatherManager()
        {
            isNight = false;
            weather = "clear";
        }

        public void daytime()
        {
            isNight = false;
            Debug.WriteLine("it is now daytime");
        }

        public void nighttime()
        {
            isNight = true;
            Debug.WriteLine("it is now nighttime");
        }

        public void clear()
        {
            weather = "clear";
            Debug.WriteLine("the weather is clear");
        }

        public void rain()
        {
            weather = "rain";
            Debug.WriteLine("it is raining");
        }
    }
}
