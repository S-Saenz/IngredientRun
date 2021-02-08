using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace IngredientRun
{
    class PickupItem : IPhysicsObject, ISpawnable
    {
        public Texture2D texture;
        private float _scale = 1;
        public Vector2 _loc { private set; get; }
        private CollisionBox _collisionBox;

        public PickupItem(string type, Vector2 position, PhysicsHandler physicsHandler)
        {
            texture = ItemTextures.GetTexture(type);
            _loc = position - new Vector2(texture.Width * _scale, texture.Height * _scale);
            _collisionBox = new CollisionBox(new RectangleF(_loc.X, _loc.Y, texture.Width * _scale, texture.Height * _scale), physicsHandler, this);
            physicsHandler.AddObject("Pickup", _collisionBox);
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
