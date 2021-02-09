using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace IngredientRun
{
    class Enemy : IPhysicsObject, ISpawnable
    {
        private Texture2D texture;
        private float _scale = 1f;
        private Vector2 _loc;
        private CollisionBox _collisionBox;
        private PhysicsHandler _collisionHandler;

        public Enemy(string type, Vector2 position, PhysicsHandler collisionHandler)
        {
            _collisionHandler = collisionHandler;

            texture = EnemyTextures.GetTexture(type);
            _loc = position - new Vector2(texture.Width * _scale, texture.Height * _scale);
            _collisionBox = new CollisionBox(new RectangleF(_loc.X, _loc.Y, texture.Width * _scale, texture.Height * _scale), _collisionHandler, this);
            _collisionHandler.AddObject("Enemy", _collisionBox);
        }

        public Vector2 GetPos()
        {
            return _loc;
        }

        public void Update(GameTime gameTime)
        {
            _collisionBox.Update(gameTime);
        }

        public void Load(ContentManager Content)
        {

        }


        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {

            spriteBatch.Draw(texture, _loc, null, Color.White, 0.0f, Vector2.Zero, _scale, SpriteEffects.None, 0.5f);

            if(isDebug)
            {
                _collisionBox.Draw(spriteBatch);
            }
        }
    }
}
