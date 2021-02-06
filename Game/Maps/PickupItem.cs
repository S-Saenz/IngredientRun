using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


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
            _loc = position;
            texture = ItemTextures.GetTexture(type);
            _collisionBox = new CollisionBox(texture.Bounds, physicsHandler, this);
            physicsHandler.AddObject("Pickup", _collisionBox);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, _loc, null, Color.White, 0.0f, Vector2.Zero, _scale, SpriteEffects.None, 0.5f);
        }
    }
}
