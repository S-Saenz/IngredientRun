using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    public class PickupItem : IPhysicsObject, ISpawnable
    {
        public string _name { get; private set; }
        public SpawnPoint _spawn { get; private set; }
        public Texture2D _texture;
        private float _scale = 1;
        public Vector2 _loc { private set; get; }
        private CollisionBox _collisionBox;

        public PickupItem(string type, Vector2 position, PhysicsHandler physicsHandler, SpawnPoint spawn = null)
        {
            _name = type;
            _spawn = spawn;
            _texture = ItemTextures.GetTexture(type);
            _loc = position - new Vector2(_texture.Width * _scale / 2, _texture.Height * _scale);
            _collisionBox = new CollisionBox(new RectangleF(_loc.X, _loc.Y, _texture.Width * _scale, _texture.Height * _scale), physicsHandler, this);
            physicsHandler.AddObject("Pickup", _collisionBox);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextureAtlasManager.DrawTexture(spriteBatch, "Item", _name, _loc, Color.White);
        }

        public bool RemoveCollision(PhysicsHandler collisionHandler)
        {
            return collisionHandler.RemoveObject(_collisionBox);
        }
    }
}
