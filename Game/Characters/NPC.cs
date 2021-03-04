using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class NPC : IPhysicsObject
    {
        float _scale = 1f;
        Texture2D _texture;
        public Vector2 _pos;
        private Vector2 _dialogueLoc;

        public NPC(Texture2D image, Vector2 position)
        {
            _texture = image;
            _pos = position;
            _dialogueLoc = new Vector2(_texture.Bounds.Width * _scale + 2, -_texture.Height * _scale - 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RectangleF dest = new RectangleF(_pos.X, _pos.Y - _texture.Height * _scale, _texture.Width * _scale, _texture.Height * _scale);
            spriteBatch.Draw(_texture, (Rectangle)dest, null, Color.White);
        }
        
        public Vector2 GetDialogueLoc(OrthographicCamera camera)
        {
            var pos = camera.WorldToScreen(_pos + _dialogueLoc); // _pos + _dialogueLoc
            return pos;
        }
    }
}