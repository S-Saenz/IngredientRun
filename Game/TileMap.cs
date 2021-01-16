using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace IngredientRun
{
    class TileMap : IPhysicsObject
    {
        TiledMap _map;
        TiledMapRenderer _renderer;
        TiledMapTileLayer _collision;
        PhysicsHandler _collisionHandler;

        public TileMap(string mapPath, ContentManager content, GraphicsDevice graphics, PhysicsHandler collisionHandler)
        {
            _map = content.Load<TiledMap>(mapPath);
            _renderer = new TiledMapRenderer(graphics, _map);

            _collision = _map.GetLayer<TiledMapTileLayer>("Walls");
            collisionHandler.AddLayer("Walls");
            foreach(TiledMapTile tile in _collision.Tiles)
            {
                if (!tile.IsBlank)
                {
                    collisionHandler.AddObject("Walls", new CollisionBox(
                        new RectangleF(tile.X * _map.TileWidth, tile.Y * _map.TileHeight, _map.TileWidth, _map.TileHeight),
                        collisionHandler, parent: this));
                }
            }
            _collisionHandler = collisionHandler;
        }

        public void Update(GameTime gameTime)
        {
            _renderer.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, Matrix projMatrix, bool isDebug = false)
        {
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _renderer.Draw(viewMatrix, projMatrix);
            if(isDebug)
            {
                _collisionHandler.Draw(spriteBatch, "Walls");
            }
            spriteBatch.End();
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
    }
}
