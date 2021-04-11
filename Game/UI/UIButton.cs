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

//written by Derek Gomez
//directly inspired by Alex Borlin's MenuButton.cs 
namespace WillowWoodRefuge
{
    class UIButton: Sprite
    {
        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private bool _isHovering;
        private Texture2D _texture;

        public event EventHandler Click;
        public bool _clicked;
        public Vector2 _position;
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            }
        }

        public bool mouseClicked()
        {
            return (_currentMouse.LeftButton == ButtonState.Released
                && _previousMouse.LeftButton == ButtonState.Pressed);
        }


        public UIButton(Texture2D texture, Vector2 pos)
        {
            this.img = texture; //Sprite.img
            this._position = pos;
            this.pos = pos;
        }

        public UIButton(Texture2D texture)
        {
            this.img = texture;
            this._texture = texture;
        }

        //this constructor will use the texture atlas
        public UIButton(String textureAtlasIThink, Vector2 pos)
        {

        }

        public void Load(ContentManager Content)
        {

        }

        public void Update(MouseState mouseState)
        {
            //Debug.WriteLine($"{this.img} is being updated!");
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            Point mousePos = mouseState.Position;

            //mouse has just started hovering
            if(!_isHovering && this.IsPointOver(mousePos))
            {
                Debug.WriteLine($"Mouse over {this.img}!");
            }
            else if(_isHovering && !this.IsPointOver(mousePos))
            {
                Debug.WriteLine($"Mouse left {this.img}!");
            }
            
            _isHovering = this.IsPointOver(mousePos);

            if (_isHovering && mouseClicked())
            {
                Debug.WriteLine("Backpack clicked!");
                //not completely sure how this works, but following Alex's lead
                Click?.Invoke(this, new EventArgs());
            }

        }
    }
}
