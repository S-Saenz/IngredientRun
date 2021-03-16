using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;
using System.Linq;

namespace WillowWoodRefuge
{
    public class TileMap : IPhysicsObject
    {
        TiledMap _map;
        TiledMapRenderer _renderer;
        PhysicsHandler _collisionHandler;

        List<SpawnPoint> _pickupSpawns;
        List<SpawnPoint> _enemySpawns;
        Dictionary<string, List<Area>> _areas;

        public RectangleF _mapBounds { private set; get; }

        public TileMap(string mapPath, ContentManager content, GraphicsDevice graphics, PhysicsHandler collisionHandler)
        {
            _map = content.Load<TiledMap>(mapPath);
            _renderer = new TiledMapRenderer(graphics, _map);

            AddWallCollision(collisionHandler);
            AddItemSpawnPoints(collisionHandler);
            AddEnemySpawnPoints(collisionHandler);
            AddAreaObjects(collisionHandler);
        }

        private void AddWallCollision(PhysicsHandler collisionHandler)
        {
            TiledMapTileLayer collision = _map.GetLayer<TiledMapTileLayer>("Walls");
            foreach (TiledMapTile tile in collision.Tiles)
            {
                if (!tile.IsBlank)
                {
                    collisionHandler.AddObject("Walls", new CollisionBox(
                        new RectangleF(tile.X * _map.TileWidth, tile.Y * _map.TileHeight, _map.TileWidth, _map.TileHeight),
                        collisionHandler, parent: this));
                }
            }
            _collisionHandler = collisionHandler;
            _mapBounds = new RectangleF(0, 0, _map.WidthInPixels, _map.HeightInPixels);
        }

        private void AddItemSpawnPoints(PhysicsHandler collisionHandler)
        {
            _pickupSpawns = new List<SpawnPoint>();
            TiledMapObjectLayer spawnPoints = _map.GetLayer<TiledMapObjectLayer>("ItemObjects");
            foreach (TiledMapObject obj in spawnPoints.Objects)
            {
                string[] vals = obj.Name.Split('.');
                string range = vals[0];
                string type = vals.Length > 1 ? vals[1] : null;
                _pickupSpawns.Add(new ItemSpawn(obj.Position, range, collisionHandler, type));
            }
        }

        private void AddEnemySpawnPoints(PhysicsHandler collisionHandler)
        {
            _enemySpawns = new List<SpawnPoint>();
            TiledMapObjectLayer spawnPoints = _map.GetLayer<TiledMapObjectLayer>("EnemyObjects");

            foreach (TiledMapObject obj in spawnPoints.Objects)
            {
                string[] vals = obj.Name.Split('.');
                string range = vals[0];
                string type = vals.Length > 1 ? vals[1] : null;
                _enemySpawns.Add(new EnemySpawn(obj.Position, range, collisionHandler, type));
            }
        }

        private void AddAreaObjects(PhysicsHandler collisionHandler)
        {
            _areas = new Dictionary<string, List<Area>>();
            TiledMapObjectLayer areaObj = _map.GetLayer<TiledMapObjectLayer>("AreaObjects");
            if(areaObj == null)
            {
                return;
            }

            foreach (TiledMapObject obj in areaObj.Objects)
            {
                if(!_areas.ContainsKey(obj.Name))
                {
                    _areas.Add(obj.Name, new List<Area>());
                }
                _areas[obj.Name].Add(new Area(collisionHandler, new RectangleF(obj.Position, obj.Size), obj.Name));
            }
        }

        public List<Area> GetAreaObject(string area)
        {
            if(_areas.ContainsKey(area))
            {
                return _areas[area];
            }
            return new List<Area>();
        }

        public void SpawnPickups(ref List<PickupItem> items)
        {
            foreach(SpawnPoint point in _pickupSpawns)
            {
                items.Add((PickupItem)point.Spawn());
            }
        }

        public void SpawnEnemies(ref List<Enemy> enemies)
        {
            foreach (SpawnPoint point in _enemySpawns)
            {
                enemies.Add((Enemy)point.Spawn());
            }
        }

        public void PlaceNPCs(Dictionary<string, NPC> characters)
        {
            // temp even spawn spacing
            float spacing = _areas["Camp"][0]._bounds.Width / 6;
            NPC[] chars = characters.Values.ToArray();

            for(int i = 1; i < 6; ++i)
            {
                chars[i - 1]._pos = new Vector2(_areas["Camp"][0]._bounds.Left + i * spacing, _areas["Camp"][0]._bounds.Bottom);
            }
        }

        public void Update(GameTime gameTime)
        {
            _renderer.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, Matrix projMatrix)
        {
            _renderer.Draw(viewMatrix, projMatrix);
        }

        public void DrawLayer(SpriteBatch spriteBatch, string name, Matrix? viewMatrix = null, Matrix? projMatrix = null)
        {
            TiledMapLayer layer = _map.GetLayer<TiledMapLayer>(name);
            if (layer != null)
            {
                _renderer.Draw(layer, viewMatrix, projMatrix);
            }
        }

        public void DrawPickups(SpriteBatch spriteBatch)
        {
            foreach(ItemSpawn obj in _pickupSpawns)
            {
                obj.Draw(spriteBatch);
            }
        }

        public void DrawEnemies(SpriteBatch spriteBatch)
        {
            foreach (EnemySpawn obj in _enemySpawns)
            {
                obj.Draw(spriteBatch);
            }
        }

        public Vector2 GetWaypoint(string layer, string name)
        {
            var objects = ((TiledMapObjectLayer)_map.GetLayer(layer)).Objects;
            for(int i = 0; i < objects.Length; ++i)
            {
                if(objects[i].Name == name)
                {
                    return objects[i].Position;
                }
            }
            return Vector2.Zero;
        }

        public NavPointMap GenerateNavPointMap(RectangleF collisionBox)
        {
            return new NavPointMap(_map, collisionBox);
        }
    }
}
