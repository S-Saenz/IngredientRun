using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace WillowWoodRefuge
{
    class Fog : WeatherElement
    {
        public float _falloff { get; protected set; }
        private Vector2 _scale = new Vector2(.5f, 2);

        public Fog(Vector2 direction, Vector2 bounds, float density, Color color, float falloff, ContentManager content) :
            base(direction, bounds, density, color)
        {
            _effect = content.Load<Effect>("shaders/Fog");
            _falloff = falloff;

            ChangeParam(direction, bounds, density, color);
            GenerateNoiseTexture();
        }

        protected override void GenerateNoiseTexture()
        {
            if (_bounds.X > 0 && _bounds.Y > 0)
            {
                // yeeeee, get that simplex!
                FastNoiseLite noiseGen = new FastNoiseLite();
                noiseGen.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

                _sourceNoise = new Texture2D(Game1.instance.GraphicsDevice, (int)_bounds.X, (int)_bounds.Y);

                // populate rain with random noise
                Color[] data = new Color[(int)_bounds.X * (int)_bounds.Y];
                for (int x = 0; x < _bounds.X; ++x)
                {
                    for (int y = 0; y < _bounds.Y; ++y)
                    {
                        int val = (int)(noiseGen.GetNoise(x * _scale.X, y * _scale.Y) * 255);
                        val = (int)(val * Math.Pow(y / _bounds.Y, _falloff));
                        data[(int)(y * _bounds.X + x)] = new Color(val, val, val, 225);
                    }
                }
                _sourceNoise.SetData(data);
            }
        }
    }
}
