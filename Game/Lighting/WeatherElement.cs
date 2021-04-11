using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace WillowWoodRefuge
{
    abstract class WeatherElement
    {
        public Vector2 _direction { get; protected set; }
        public Vector2 _bounds { get; protected set; }
        public float _density { get; protected set; }
        public Color _color { get; protected set; }

        protected Vector2 _movement = Vector2.Zero;
        protected Texture2D _sourceNoise;
        protected Effect _effect;

        public WeatherElement(Vector2 direction, Vector2 bounds, float density, Color color)
        {
            _direction = direction;
            _bounds = bounds;
            _density = density;
            _color = color;
        }

        public void Update(GameTime gameTime)
        {
            _movement -= _direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_movement.X < 0)
                _movement.X += _bounds.X;
            if (_movement.Y < 0)
                _movement.Y += _bounds.Y;
            _effect.Parameters["Movement"].SetValue(_movement);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(blendState: BlendState.Additive, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _effect);
            spriteBatch.Draw(_sourceNoise, Vector2.Zero, _color);
            spriteBatch.End();

            // Stream stream = File.Create("rain.png");
            // _sourceNoise.SaveAsPng(stream, _sourceNoise.Width, _sourceNoise.Height);
            // stream.Dispose();
        }

        public void ChangeParam(Vector2? direction = null, Vector2? bounds = null, float density = -1, Color? color = null)
        {
            if (direction.HasValue)
            {
                _direction = direction.Value;
            }
            if(bounds.HasValue)
            {
                _bounds = bounds.Value;
                _effect.Parameters["TextureDimensions"].SetValue(new Vector2(_bounds.X, _bounds.Y));
                GenerateNoiseTexture();
            }
            if(density >= 0)
            {
                _density = density;
                _effect.Parameters["Density"].SetValue(_density);
            }
            if(color.HasValue)
            {
                _color = color.Value;
            }
        }

        protected abstract void GenerateNoiseTexture();
    }
}
