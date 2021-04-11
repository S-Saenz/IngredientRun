using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace WillowWoodRefuge
{
    class Rain : WeatherElement
    {
        public Rain(Vector2 direction, Vector2 bounds, float density, Color color, ContentManager content) : 
            base(direction, bounds, density, color)
        {
            _effect = content.Load<Effect>("shaders/Rain");

            ChangeParam(direction, bounds, density, color);
            GenerateNoiseTexture();
        }

        protected override void GenerateNoiseTexture()
        {
            if (_bounds.X > 0 && _bounds.Y > 0)
            {
                _sourceNoise = new Texture2D(Game1.instance.GraphicsDevice, (int)_bounds.X, (int)_bounds.Y);

                // populate rain with random noise
                Color[] data = new Color[(int)_bounds.X * (int)_bounds.Y];
                Random _rand = new Random();
                for (int i = 0; i < data.Length; ++i)
                {
                    int val = _rand.Next(0, 255);
                    data[i] = new Color(val, val, val, 225);
                }
                _sourceNoise.SetData(data);
            }
        }
    }
}
