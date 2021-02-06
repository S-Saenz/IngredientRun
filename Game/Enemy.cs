using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;

namespace IngredientRun
{
    class Enemy : IPhysicsObject
    {
        private Texture2D texture;
        private float _scale = 1.5f;
        private Vector2 pos;
        private Vector2 staticPos;
        private CollisionBox _collisionBox;
        private PhysicsHandler _collisionHandler;

        public Enemy(Texture2D img, Vector2 position, PhysicsHandler collisionHandler)
        {
            pos = staticPos = position;
            texture = img;

            _collisionHandler = collisionHandler;
        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public void Update(GameTime gameTime)
        {
            // pos = mapPos + staticPos;
            // hitBox = new Rectangle(new Point((int)(mapPos.X + pos.X), (int)(mapPos.Y + pos.Y)), new Point(texture.Height, texture.Width));
            _collisionBox.Update(gameTime);
        }

        public void Load(ContentManager Content)
        {
            // hitBox = new Rectangle(new Point(100, 200), new Point(texture.Height, texture.Width));
            pos.Y -= texture.Height * _scale;
            pos.X -= texture.Width * _scale / 2;

            _collisionBox = new CollisionBox(new RectangleF(pos,
                new Size2(texture.Bounds.Width * _scale, texture.Bounds.Height * _scale)),
                _collisionHandler, this);
            _collisionHandler.AddObject("Enemy", _collisionBox);
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, pos, null, Color.White, 0.0f, Vector2.Zero, _scale, SpriteEffects.None, 0.5f);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }
    }
}
