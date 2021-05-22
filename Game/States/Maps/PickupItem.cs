using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    class PickupItem : IPhysicsObject
    {
        public string _name { get; private set; }
        public CollisionBox _collisionBox { get; private set; }
        static protected List<PickupItem> _items = new List<PickupItem>();
        static public List<PickupItem> _pickupItems { get { return _items; } }
        protected List<PickupItem> _sceneItems;

        // Timer variables
        static float _despawnDuration = 10; // how many seconds before dropped item despawns
        public float _timeElapsed { get; protected set; } // current time on clock
        private bool _isPaused = false;


        public PickupItem(string name, Vector2 loc, PhysicsHandler physicsHandler, ref List<PickupItem> sceneItems)
        {
            _name = name;
            _collisionBox = new CollisionBox(new RectangleF(loc, TextureAtlasManager.GetSize("PickupItems", _name)), physicsHandler, parent: this);
            physicsHandler.AddObject("PickupItems", _collisionBox);
            _timeElapsed = 0;
            _sceneItems = sceneItems;
            _items.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            _timeElapsed += gameTime.GetElapsedSeconds();
            if (_timeElapsed >= _despawnDuration)
                _items.Remove(this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextureAtlasManager.DrawTexture(spriteBatch, "PickupItems", _name, _collisionBox._bounds.Position, Color.White);
        }

        // returns the number of inventory spaces taken by item. all items take 1 space for now
        public int GetSize()
        {
            return 1;
        }

        // despawns item and returns the name of the item to add to inventory
        public string Pickup()
        {
            _items.Remove(this);
            return _name;
        }
    }
}
