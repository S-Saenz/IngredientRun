using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Diagnostics;

namespace WillowWoodRefuge
{
    public delegate void WindowResizeEventHandler(Vector2 size);

    public class CameraController
    {
        public Vector2 _screenRatio { get; private set; }
        public Vector2 _cameraOffset { get; private set; }
        public RectangleF? _worldBounds { get; private set; }
        public RectangleF? _playerBounds { get; private set; }
        public OrthographicCamera _camera { get; private set; }
        private GraphicsDeviceManager _graphics;
        public Vector2 _pixelDimensions { get; private set; }
        public Vector2 _screenDimensions { get; private set; }
        private Vector2 _windowDimensions;
        private Vector2 _oldPoint = Vector2.Zero;

        private event WindowResizeEventHandler _onResize;

        public CameraController(GraphicsDeviceManager graphics, Vector2 screenRatio, Vector2 pixelDimensions, Vector2 screenDimensions)
        {
            _graphics = graphics;
            _screenRatio = screenRatio;
            _pixelDimensions = pixelDimensions;
            _windowDimensions = screenDimensions;

            // setup the window
            RecalculateScreenDimensions(screenDimensions);
        }

        public void Update(GameTime gameTime, Vector2 loc)
        {
            Vector2 newPos = loc;
            _oldPoint = newPos;
            if (_playerBounds.HasValue)
            {
                RectangleF playBounds = _playerBounds.Value;
                RectangleF camBounds = _camera.BoundingRectangle;
                // set playBounds to be centered in camera
                playBounds.Position = camBounds.Center - (Vector2)playBounds.Size / 2;
                _playerBounds = playBounds;

                if(playBounds.Left <= newPos.X && playBounds.Right >= newPos.X) // inside width bounds
                {
                    newPos.X = playBounds.Center.X;
                }
                else
                {
                    if (newPos.X < playBounds.Left) // out left
                    {
                        newPos.X += playBounds.Width / 2;
                    }
                    else if (newPos.X > playBounds.Right) // out right
                    {
                        newPos.X -= playBounds.Width / 2;
                    }
                }

                if (playBounds.Top <= newPos.Y && playBounds.Bottom >= newPos.Y) // inside height bounds
                {
                    newPos.Y = playBounds.Center.Y;
                }
                else
                {
                    if (newPos.Y < playBounds.Top) // out top
                    {
                        newPos.Y += playBounds.Height / 2;
                    }
                    else if (newPos.Y > playBounds.Bottom) // out bottom
                    {
                        newPos.Y -= playBounds.Height / 2;
                    }
                }
            }

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
            // parallax when in camp state
            
            _camera.LookAt(newPos);
            if (_worldBounds.HasValue)
            {
                float worldRight = _worldBounds.Value.Right;
                // _worldBounds.Value.Center.X;
                // _camera.BoundingRectangle;
                Vector2 offset;
                offset.X = 1 - 2 * (worldRight - _camera.BoundingRectangle.Right) / (worldRight - _camera.BoundingRectangle.Width);
                // if vertical parrax is needed at some point, this will need actual logic
                offset.Y = 0;//1 - 2 * (worldRight - _camera.BoundingRectangle.Right) / (worldRight - _camera.BoundingRectangle.Width);
                _cameraOffset = offset;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_worldBounds.HasValue)
            {
                spriteBatch.DrawRectangle(_worldBounds.Value, Color.Purple);
            }
            if (_playerBounds.HasValue)
            {
                spriteBatch.DrawRectangle(_playerBounds.Value, Color.Orange);
                spriteBatch.DrawPoint(_oldPoint, Color.HotPink, 5);
            }

            spriteBatch.DrawRectangle(_camera.BoundingRectangle, Color.Red);
        }

        public void SetWorldBounds(RectangleF worldBounds)
        {
            // calculate dimensions restrained by screenDimensions (so that even if world bounds are smaller
            // than height of screen display, boundswidth >= screenwidth, boundsheight >= screenheight, and
            // bottom edge of bounds is never expanded beyond input (if it needs to be resized)
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

        public void SetPlayerBounds(RectangleF playerBounds)
        {
            // makes sure bounds are smaller than camera view
            if(playerBounds.Width  < _pixelDimensions.X &&
               playerBounds.Height < _pixelDimensions.Y)
            {
                playerBounds.Position = _pixelDimensions / 2 - (Vector2)playerBounds.Size / 2;
                _playerBounds = playerBounds;
            }
        }

        private void RecalculateScreenDimensions(Vector2 screenDimensions, Vector2? pos = null)
        {
            // calculate dimensions restrained by screenRatio
            // float widthScale  = screenDimensions.X / _screenRatio.X;
            // float heightScale = screenDimensions.Y / _screenRatio.Y;
            // if (widthScale < heightScale) // limited by width
            // {
            //     screenDimensions.Y = widthScale * _screenRatio.Y;
            // }
            // else // limited by height
            // {
            //     screenDimensions.X = heightScale * _screenRatio.X;
            // }

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
            _onResize?.Invoke(_screenDimensions);
        }

        public Matrix GetViewMatrix()
        {
            return _camera.GetViewMatrix();
        }

        public void MakeFullScreen()
        {
            RecalculateScreenDimensions(new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height));
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        public void MakeWindowed()
        {
            RecalculateScreenDimensions(_windowDimensions);
            _graphics.IsFullScreen = false; // set true to default later
            _graphics.ApplyChanges();
        }

        public void ToggleFullscreen()
        {
            if(_graphics.IsFullScreen == true)
            {
                RecalculateScreenDimensions(_windowDimensions);
                _graphics.IsFullScreen = false; // set true to default later
            }
            else
            {
                RecalculateScreenDimensions(new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height));
                _graphics.IsFullScreen = true;
            }
            _graphics.ApplyChanges();
        }

        public void AddResizeListener(WindowResizeEventHandler resizeEvent)
        {
            _onResize += resizeEvent;
        }
    }
}
