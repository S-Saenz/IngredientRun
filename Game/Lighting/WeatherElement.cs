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
        private float _scale = 1;

        protected Vector2 _movement = Vector2.Zero;
        protected Texture2D _sourceNoise;
        public RenderTarget2D _texture;

        protected Effect _scroll;
        protected Effect _effect;

        public WeatherElement(Vector2 direction, Vector2 bounds, float density, Color color, ContentManager content)
        {
            _direction = direction;
            _bounds = bounds;
            _density = density;
            _color = color;

            if(bounds.X > 0 && bounds.Y > 0)
                _texture = new RenderTarget2D(
                    Game1.instance.GraphicsDevice,
                    (int)bounds.X,
                    (int)bounds.Y,
                    false,
                    Game1.instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24);

            _scroll = content.Load<Effect>("shaders/TextureWrap");
        }

        public void Update(GameTime gameTime)
        {
            _movement -= _direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_movement.X < 0)
                _movement.X += _bounds.X;
            else if (_movement.X > _bounds.X)
                _movement.X -= _bounds.X;
            if (_movement.Y < 0)
                _movement.Y += _bounds.Y;
            else if (_movement.Y > _bounds.Y)
                _movement.Y -= _bounds.Y;
        }

        public void Generate(SpriteBatch spriteBatch)
        {
            if (_bounds.X > 0 && _bounds.Y > 0)
            {
                Game1.instance.GraphicsDevice.SetRenderTarget(_texture);
                Game1.instance.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(blendState: BlendState.Additive, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _effect);
                spriteBatch.Draw(_sourceNoise, Vector2.Zero, _color);
                spriteBatch.End();
                Game1.instance.GraphicsDevice.SetRenderTarget(null);
            }

            // Stream stream = File.Create("rain.png");
            // _sourceNoise.SaveAsPng(stream, _sourceNoise.Width, _sourceNoise.Height);
            // stream.Dispose();
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            if (_texture != null)
            {
                _scroll.Parameters["SourceTexture"].SetValue(_texture);
                _scroll.Parameters["SourceDimensions"].SetValue(new Vector2(_texture.Width, _texture.Height));
                _scroll.Parameters["DestinationDimensions"].SetValue(_bounds);
                _scroll.Parameters["Scale"].SetValue(_scale);
                _scroll.Parameters["Position"].SetValue(_movement);
                spriteBatch.Begin(transformMatrix: viewMatrix, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _scroll);
                spriteBatch.Draw(_texture, Vector2.Zero, _color);
                spriteBatch.End();
            }
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
                if (_bounds.X > 0 && _bounds.Y > 0)
                    _texture = new RenderTarget2D(
                        Game1.instance.GraphicsDevice,
                        (int)_bounds.X,
                        (int)_bounds.Y,
                        false,
                        Game1.instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        DepthFormat.Depth24);
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

            Generate(Game1.instance._spriteBatch);
        }

        protected abstract void GenerateNoiseTexture();
    }
}
