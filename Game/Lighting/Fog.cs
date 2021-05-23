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
        public int _steps { get; protected set; }
        private Vector2 _scale = new Vector2(400, 1200);

        public Fog(Vector2 direction, Vector2 bounds, float density, Color color, float falloff, int steps, ContentManager content, SpriteBatch spriteBatch) :
            base(direction, bounds, density, color, content, spriteBatch)
        {
            _effect = content.Load<Effect>("shaders/Fog");
            _falloff = falloff;
            _steps = steps;

            ChangeParam(direction, bounds, density, color, falloff, steps);
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
                        float s = x / _bounds.X;
                        float t = y / _bounds.Y;
                        float dx = _scale.X;
                        float dy = _scale.Y;
                        float nx = -_scale.X + (float)(Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI));
                        float ny = -_scale.Y + (float)(Math.Cos(t * 2 * Math.PI) * dy / (2 * Math.PI));
                        float nz = -_scale.X + (float)(Math.Sin(s * 2 * Math.PI) * dx / (2 * Math.PI));
                        int val = (int)(noiseGen.GetNoise(nx, ny, nz) * 255);
                        data[(int)(y * _bounds.X + x)] = new Color(val, val, val, 225);
                    }
                }
                _sourceNoise.SetData(data);
            }
        }

        public void ChangeParam(Vector2? direction = null, Vector2? bounds = null, float density = -1, Color? color = null, float? falloff = null, int? steps = null)
        {
            if(falloff.HasValue)
            {
                _falloff = falloff.Value;
                _effect.Parameters["Falloff"].SetValue(_falloff);
            }
            if (steps.HasValue)
            {
                _steps = steps.Value;
                _effect.Parameters["Steps"].SetValue(_steps);
            }
            base.ChangeParam(direction, bounds, density, color);
        }
    }
}
