using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientRun
{
    class EnemySpawn : SpawnPoint
    {
        public EnemySpawn(Vector2 loc, string rangeType, PhysicsHandler physicsHandler, string spawnType = null) : 
            base(loc, rangeType, physicsHandler, spawnType)
        {
            
        }

        public override ISpawnable Spawn()
        {
            Despawn();
            _isSpawned = true;

            switch (_rangeType)
            {
                case "only":
                    _object = new Enemy(_spawnType, _location, _physicsHandler);
                    break;
                case "family":
                    break;
                case "any":
                    string spawnType = EnemyTextures._allItems[new Random().Next(EnemyTextures._allItems.Count)];
                    _object = new Enemy(spawnType, _location, _physicsHandler);
                    break;
            }
            return _object;
        }

        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            if(_object != null)
            {
                _object.Draw(spriteBatch, isDebug);
            }
        }
    }
}
