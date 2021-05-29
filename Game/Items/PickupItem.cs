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
        private PhysicsHandler _physicsHandler;

        // Timer variables
        static float _despawnDuration = 10; // how many seconds before dropped item despawns
        public float _timeElapsed { get; protected set; } // current time on clock
        private bool _decays = false;

        // Container for all dropped items in game (key is scene, value is list of pickup items in scene)
        static protected Dictionary<string, List<PickupItem>> _items = new Dictionary<string, List<PickupItem>>();
        static public Dictionary<string, List<PickupItem>> _pickupItems { get { return _items; } }
        private string _scene;

        public PickupItem(string name, Vector2 loc, PhysicsHandler physicsHandler, string scene, bool decays = true)
        {
            _name = name;
            _collisionBox = new CollisionBox(new RectangleF(loc - new Vector2(8, 16), new Vector2(16)), physicsHandler, parent: this);
            physicsHandler.AddObject("PickupItems", _collisionBox);
            _physicsHandler = physicsHandler;
            _timeElapsed = 0;
            _scene = scene;
            if (!_items.ContainsKey(scene))
                _items.Add(scene, new List<PickupItem>());
            _items[scene].Add(this);
            _decays = decays;
        }

        static public void UpdateAll(GameTime gameTime)
        {
            foreach (List<PickupItem> list in _items.Values)
            {
                for(int i = list.Count - 1; i >= 0 && list.Count > 0; --i)
                    list[i].Update(gameTime);
            }
        }

        private void Update(GameTime gameTime)
        {
            if (_decays)
            {
                _timeElapsed += gameTime.GetElapsedSeconds();
                if (_timeElapsed >= _despawnDuration)
                    Pickup();
            }
        }

        static public void DrawAll(SpriteBatch spriteBatch, string scene)
        {
            if (_items.ContainsKey(scene))
                foreach (PickupItem item in _items[scene])
                    item.Draw(spriteBatch);
        }

        private void Draw(SpriteBatch spriteBatch)
        {
            TextureAtlasManager.DrawTexture(spriteBatch, "Item", _name, (Rectangle)_collisionBox._bounds, Color.White);
        }

        // despawns item and returns the name of the item to add to inventory
        public string Pickup()
        {
            _items[_scene].Remove(this);
            _physicsHandler.RemoveObject(this._collisionBox);
            return _name;
        }
    }
}
