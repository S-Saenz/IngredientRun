using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientRun
{
    class NPC : IPhysicsObject
    {
        float _scale = 1.5f;
        Texture2D _texture;
        public Vector2 _pos;

        public NPC(Texture2D image, Vector2 position)
        {
            _texture = image;
            _pos = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RectangleF dest = new RectangleF(_pos.X - _texture.Width * _scale, _pos.Y - _texture.Height * _scale, _texture.Width * _scale, _texture.Height * _scale);
            spriteBatch.Draw(_texture, (Rectangle)dest, null, Color.White);
        }
    }
}