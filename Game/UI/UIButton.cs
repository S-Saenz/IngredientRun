using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

//written by Derek Gomez
//directly inspired by Alex Borlin's MenuButton.cs 
namespace WillowWoodRefuge
{
    class UIButton
    {
        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private bool _isHovering;
        
        public Texture2D _texture = null;

        private string _name;
        private string _text = "";

        public float _scale = 1;
        public Vector2 _position;
        public Rectangle _rectangle;
        bool _isCentered;

        public event EventHandler Click;
        public bool _clicked;

        private float screenScale = 1; //just a local copy of cameraController._screenScale

        public bool mouseClicked()
        {
            return (_currentMouse.LeftButton == ButtonState.Released
                && _previousMouse.LeftButton == ButtonState.Pressed);
        }

        //we used these when we inherited from sprite
        public UIButton(Texture2D texture, Vector2 pos, string text = "")
        {
            //this.img = texture; //Sprite.img
            this._position = pos;
            this._texture = texture;
            _text = text;
            //this.pos = pos
            Game1.instance._cameraController.AddResizeListener(onResize);
        }
        

        public UIButton(Texture2D texture)
        {
            //this.img = texture;
            this._texture = texture;
            Game1.instance._cameraController.AddResizeListener(onResize);
        }
        

        //this constructor will use the texture atlas
        public UIButton(string textureName, Vector2 pos, string text = "", bool isCentered = false)
        {
            this._name = textureName;
            this._position = pos;
            _text = text;
            _isCentered = isCentered;

            Size2 textureSize = TextureAtlasManager.GetSize("UI", _name);
            screenScale = Game1.instance._cameraController._screenScale;
            this._rectangle = new Rectangle((int)(pos.X * screenScale), (int)(pos.Y * screenScale), (int)(textureSize.Width * screenScale *_scale), (int)(textureSize.Height * screenScale *_scale));
            if(isCentered)
            {
                _rectangle.X -= _rectangle.Width / 2;
                _rectangle.Y -= _rectangle.Height / 2;
            }

            Game1.instance._cameraController.AddResizeListener(onResize);
        }

        //when window is resized, also resize button's position and size!
        //new screen dimensions are passed in as arguments?
        void onResize(Vector2 size)
        {
            screenScale = Game1.instance._cameraController._screenScale;

            Size2 textureSize;
            if (_texture == null)
                textureSize = TextureAtlasManager.GetSize("UI", _name);
            else
                textureSize = _texture.Bounds.Size;
            this._rectangle = new Rectangle((int)(_position.X * screenScale), (int)(_position.Y * screenScale), (int)(textureSize.Width * screenScale *_scale), (int)(textureSize.Height * screenScale *_scale));
            if (_isCentered)
            {
                _rectangle.X -= _rectangle.Width / 2;
                _rectangle.Y -= _rectangle.Height / 2;
            }
        }


        public void reScale(float newScale)
        {
            this._scale = newScale;
            Size2 textureSize;
            if (_texture == null)
                textureSize = TextureAtlasManager.GetSize("UI", _name);
            else
                textureSize = _texture.Bounds.Size;
            this._rectangle = new Rectangle((int)(_position.X * screenScale), (int)(_position.Y * screenScale), (int)(textureSize.Width * screenScale * _scale), (int)(textureSize.Height * screenScale * _scale));

            if (_isCentered)
            {
                _rectangle.X -= _rectangle.Width / 2;
                _rectangle.Y -= _rectangle.Height / 2;
            }
        }

        public void Load(ContentManager Content)
        {

        }

        //Rectangle.contains(texture rectangle, mouse position);
        public void Update(MouseState mouseState)
        {
            //Debug.WriteLine($"{this.img} is being updated!");
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            Point mousePos = mouseState.Position;

            //mouse has just started hovering
            if (!_isHovering && _rectangle.Contains(mousePos))
            {
                Debug.WriteLine($"Mouse over {this._texture}!");
            }
            else if (_isHovering && !_rectangle.Contains(mousePos))
            {
                Debug.WriteLine($"Mouse left {this._texture}!");
            }

            _isHovering = _rectangle.Contains(mousePos);

            if (_isHovering && mouseClicked())
            {
                //Debug.WriteLine("Backpack clicked!");
                //not completely sure how this works, but following Alex's lead
                Click?.Invoke(this, new EventArgs());
                Game1.instance.sounds.buttonSound();
            }

        }

        //add draw function for texture atlas
        public void Draw(SpriteBatch spriteBatch)
        {
            Color colour = Color.White;

            if (_isHovering)
                colour = Color.Gray;

            if (_texture == null)
                TextureAtlasManager.DrawTexture(spriteBatch, "UI", _name, _rectangle, colour);
            else
                spriteBatch.Draw(_texture, _rectangle, null, colour);
            if (_text.Length > 0)
                FontManager.PrintText(FontManager._dialogueFont, spriteBatch, _text, (Point2)_rectangle.Center, Alignment.Centered, Color.Black, true);
            // spriteBatch.Draw(img, pos, null, Color.White, Rotation, Origin, scale, SpriteEffects.None, 1f);
            // spriteBatch.DrawRectangle(_rectangle, Color.Red);
        }
    }   
}
