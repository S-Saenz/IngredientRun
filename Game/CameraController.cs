using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Diagnostics;

namespace IngredientRun
{
    public class CameraController
    {
        public Vector2 _screenRatio { get; private set; }
        public RectangleF? _worldBounds { get; set; }
        public OrthographicCamera _camera { get; private set; }
        private GraphicsDeviceManager _graphics;
        public Vector2 _pixelDimensions { get; private set; }
        public Vector2 _screenDimensions { get; private set; }

        public CameraController(GraphicsDeviceManager graphics, Vector2 screenRatio, Vector2 pixelDimensions, Vector2 screenDimensions)
        {
            _graphics = graphics;
            _screenRatio = screenRatio;
            _pixelDimensions = pixelDimensions;

            // setup the window
            RecalculateScreenDimensions(screenDimensions);
        }

        public void Update(GameTime gameTime, Vector2 loc)
        {
            Vector2 newPos = loc;
            if (_worldBounds.HasValue)
            {
                _camera.LookAt(newPos);
                RectangleF camBounds = _camera.BoundingRectangle;
                if(camBounds.Left < _worldBounds.Value.Left) // out left
                {
                    newPos.X = _worldBounds.Value.Left + _pixelDimensions.X / 2;
                }
                else if(camBounds.Right > _worldBounds.Value.Right) // out right
                {
                    newPos.X = _worldBounds.Value.Right - _pixelDimensions.X / 2;
                }
                if (camBounds.Top < _worldBounds.Value.Top) // out top
                {
                    newPos.Y = _worldBounds.Value.Top + _pixelDimensions.Y / 2;
                }
                else if (camBounds.Bottom > _worldBounds.Value.Bottom) // out bottom
                {
                    newPos.Y = _worldBounds.Value.Bottom - _pixelDimensions.Y / 2;
                }
            };
            _camera.LookAt(newPos);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(_worldBounds.Value, Color.Purple);
            spriteBatch.DrawRectangle(_camera.BoundingRectangle, Color.Red);
        }

        public void SetWorldBounds(RectangleF worldBounds)
        {
            // calculate dimensions restrained by screenDimensions (so that even if world bounds are smaller
            // than height of screen display, boundswidth >= screenwidth, boundsheight >= screenheight, and
            // bottom edge of bounds is never expanded beyond input (if it needs to be resized)
            float heightScale = _screenDimensions.Y / worldBounds.Height;
            if (_pixelDimensions.Y > worldBounds.Height) // screen height exceeds worldbounds height
            {
                float newHeight = _pixelDimensions.Y;
                worldBounds.Y -= newHeight - worldBounds.Height;
                worldBounds.Height = newHeight;
            }
            else if(_pixelDimensions.X > worldBounds.Width)
            {
                worldBounds.Width = _pixelDimensions.X;
            }

            _worldBounds = worldBounds;
        }

        private void RecalculateScreenDimensions(Vector2 screenDimensions, Vector2? pos = null)
        {
            // calculate dimensions restrained by screenRatio
            float widthScale  = screenDimensions.X / _screenRatio.X;
            float heightScale = screenDimensions.Y / _screenRatio.Y;
            if (widthScale < heightScale) // limited by width
            {
                screenDimensions.Y = widthScale * _screenRatio.Y;
            }
            else // limited by height
            {
                screenDimensions.X = heightScale * _screenRatio.X;
            }

            // apply new screen dimensions to graphics device
            _graphics.PreferredBackBufferWidth  = (int)screenDimensions.X;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = (int)screenDimensions.Y;   // set this value to the desired height of your window
            _graphics.ApplyChanges();
            _screenDimensions = screenDimensions;

            // calculate zoom based on screen dimensions and pixel dimensions
            float zoom = _screenDimensions.X / _pixelDimensions.X;

            // save old camera specs if not being overriden
            if (!pos.HasValue && _camera != null)
            {
                pos = _camera.Position;
            }

            // setup the camera
            DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(_graphics.GraphicsDevice);
            _camera = new OrthographicCamera(viewportAdapter);

            // apply new zoom
            _camera.Zoom = zoom;

            // reapply old specs, if they were defined
            if (pos.HasValue)
            {
                _camera.Position = pos.Value;
            }
        }
    }
}
