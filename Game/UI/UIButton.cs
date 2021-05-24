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

        public float _scale = 1;
        public Vector2 _position;
        public Rectangle _rectangle;

        public event EventHandler Click;
        public bool _clicked;

        private float screenScale; //just a local copy of cameraController._screenScale

        public bool mouseClicked()
        {
            return (_currentMouse.LeftButton == ButtonState.Released
                && _previousMouse.LeftButton == ButtonState.Pressed);
        }

        //we used these when we inherited from sprite
        public UIButton(Texture2D texture, Vector2 pos)
        {
            //this.img = texture; //Sprite.img
            this._position = pos;
            this._texture = texture;
            //this.pos = pos
        }
        

        public UIButton(Texture2D texture)
        {
            //this.img = texture;
            this._texture = texture;
        }
        

        //this constructor will use the texture atlas
        public UIButton(String textureName, Vector2 pos)
        {
            this._name = textureName;
            this._position = pos;

            Size2 textureSize = TextureAtlasManager.GetSize("UI", _name);
            screenScale = Game1.instance._cameraController._screenScale;
            this._rectangle = new Rectangle((int)(pos.X * screenScale), (int)(pos.Y * screenScale), (int)(textureSize.Width * screenScale *_scale), (int)(textureSize.Height * screenScale *_scale));


            Game1.instance._cameraController.AddResizeListener(onResize);
        }

        //when window is resized, also resize button's position and size!
        //new screen dimensions are passed in as arguments?
        void onResize(Vector2 size)
        {
            screenScale = Game1.instance._cameraController._screenScale;

            Size2 textureSize = TextureAtlasManager.GetSize("UI", _name);
            this._rectangle = new Rectangle((int)(_position.X * screenScale), (int)(_position.Y * screenScale), (int)(textureSize.Width * screenScale *_scale), (int)(textureSize.Height * screenScale *_scale));
        }


        public void reScale(float newScale)
        {
            this._scale = newScale;
            Size2 textureSize = TextureAtlasManager.GetSize("UI", _name);
            this._rectangle = new Rectangle((int)(_position.X * screenScale), (int)(_position.Y * screenScale), (int)(textureSize.Width * screenScale * _scale), (int)(textureSize.Height * screenScale * _scale));
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
            }

        }

        //add draw function for texture atlas
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture == null)
                TextureAtlasManager.DrawTexture(spriteBatch, "UI", _name, _rectangle, Color.White);
            else
                spriteBatch.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero, (_scale * Game1.instance._cameraController._screenScale), SpriteEffects.None, 0.01f);
            // spriteBatch.Draw(img, pos, null, Color.White, Rotation, Origin, scale, SpriteEffects.None, 1f);
        }
    }   
}
