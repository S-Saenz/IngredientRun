using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WillowWoodRefuge
{
    public interface ISpawnable
    {
        void Draw(SpriteBatch spriteBatch);
        bool RemoveCollision(PhysicsHandler physicsHandler);
    }

    public abstract class SpawnPoint
    {
        public Vector2 _location { protected set; get; }
        public string _rangeType { protected set; get; } // Any, Subset, Exact
        public string _spawnType { protected set; get; }  // Acorn, Outdoor, null(for any)
        public bool _isSpawned { protected set; get; } // whether item is currently spawned on point or not (respawn countdown should be zero for pickup if true)
        protected ISpawnable _object; // the actual spawned object, null if _isSpawned = false
        
        protected PhysicsHandler _physicsHandler;
        
        public SpawnPoint(Vector2 loc, string rangeType, PhysicsHandler physicsHandler, string spawnType = null)
        {
            _location = loc;
            _rangeType = rangeType;
            _physicsHandler = physicsHandler;
            _spawnType = spawnType;
            _isSpawned = false;
        }

        abstract public ISpawnable Spawn();

        public void Despawn()
        {
            if (_object != null)
            {
                _object.RemoveCollision(_physicsHandler);
                GameplayState state = Game1.instance._currentState as GameplayState;
                if(state == null)
                {
                    state = Game1.instance._nextState as GameplayState;
                }

                if (_object as SpawnItem != null)
                {
                    state._spawnItems.Remove(_object as SpawnItem);
                }
                else if(_object as Enemy != null)
                {
                    state._enemies.Remove(_object as Enemy);
                }
            }
            _object = null;
            _isSpawned = false;
        }
    }
}
